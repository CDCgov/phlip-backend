using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/cleanup/")]
    [Authorize]
    public class CleanupController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CleanupController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        
        [HttpDelete("deleteallanswersfromcodedquestion/", Name = "DeleteAllCodedAnswersFromCodedQuestion")]
        public async Task<IActionResult> DeleteAllCodedAnswersFromCodedQuestion([FromBody] CleanupCodedQuestionDto dto)
        {
            if (dto == null) return BadRequest();
            
            await deleteCodedAnswersFromCodedQuestion(dto);

            return await _unitOfWork.Complete() <= 0 ? StatusCode(500) : NoContent();
        }

        private async Task<bool> deleteCodedAnswersFromCodedQuestion(CleanupCodedQuestionDto dto)
        {
            if (dto.Categories.Count > 0)
            {
                foreach (var categoryId in dto.Categories)
                {
                    var ccq = await _unitOfWork.CodedCategoryQuestions
                        .SingleOrDefaultAsync(cq => 
                            cq.CodedBy.Id == dto.UserId && 
                            cq.ProjectJurisdiction.Id == dto.ProjectJurisdictionId &&
                            cq.SchemeQuestionId == dto.SchemeQuestionId &&
                            cq.Category.Id == categoryId);

                    if (ccq?.CodedAnswers?.Count <= 0) return false;
                    
                    _unitOfWork.CodedAnswers.RemoveRange(ccq.CodedAnswers);
                }
            }
            else
            {
                var codedQuestion = await _unitOfWork.CodedQuestions
                    .SingleOrDefaultAsync(
                        cq => cq.CodedBy.Id == dto.UserId &&
                        cq.ProjectJurisdiction.Id == dto.ProjectJurisdictionId &&
                        cq.SchemeQuestionId == dto.SchemeQuestionId);
                if (codedQuestion?.CodedAnswers?.Count <= 0) return false;
                
                _unitOfWork.CodedAnswers.RemoveRange(codedQuestion.CodedAnswers);

            }

            return true;
        }
        

        [HttpDelete("deletecodedquestion")]
        public async Task<IActionResult> DeleteCodedQuestion([FromBody] CleanupCodedQuestionDto dto)
        {
            if (dto == null) return BadRequest();

            //Must delete coded answers first
            await deleteCodedAnswersFromCodedQuestion(dto);
            
            if (dto.Categories.Count > 0)
            {
                foreach (var categoryId in dto.Categories)
                {
                    var codedCategoryQuestion = await _unitOfWork.CodedCategoryQuestions
                        .SingleOrDefaultAsync(ccq =>
                                ccq.Id == dto.UserId &&
                                ccq.ProjectJurisdiction.Id == dto.ProjectJurisdictionId &&
                                ccq.SchemeQuestionId == dto.SchemeQuestionId &&
                                ccq.Category.Id == categoryId);
                    if (codedCategoryQuestion != null)
                    {
                        _unitOfWork.CodedCategoryQuestions.Remove(codedCategoryQuestion);
                    }
                }
            }
            else
            {
                var codedQuestion = await _unitOfWork.CodedQuestions
                    .SingleOrDefaultAsync(cq => 
                            cq.CodedBy.Id == dto.UserId &&
                            cq.ProjectJurisdiction.Id == dto.ProjectJurisdictionId &&
                            cq.SchemeQuestionId == dto.SchemeQuestionId);
                if (codedQuestion != null)
                {
                    _unitOfWork.CodedQuestions.Remove(codedQuestion);
                }
            }

            return await _unitOfWork.Complete() <= 0 ? StatusCode(500) : NoContent();
        }

        [HttpDelete("deleteallcodedquestionsandanswers")]
        public async Task<IActionResult> DeleteAllCodedQuestionsAndAnswers()
        {
            _unitOfWork.CodedAnswers.RemoveRange(await _unitOfWork.CodedAnswers.GetAllAsync());
            _unitOfWork.CodedQuestionFlags.RemoveRange(await _unitOfWork.CodedQuestionFlags.GetAllAsync());
            _unitOfWork.CodedQuestionBases.RemoveRange(await _unitOfWork.CodedQuestionBases.GetAllAsync());
            
            return await _unitOfWork.Complete() <= 0 ? StatusCode(500) : NoContent();
        }
        [HttpDelete("{docId}/annotations")]
        public async Task<IActionResult> DeleteAllAnnotationsFromCodedAnswer(string docId)
        {
            var affectedAnswers = await _unitOfWork.CodedAnswers.FindAsync(b => b.Annotations.Contains(docId)); // get list of answers with matching documentid
            if (affectedAnswers.Count > 0)
            {
                foreach (var answer in affectedAnswers)
                {
                    // remove the json string with the documentId from annotation array
                    try
                    {
                        var cleanedAnnotations = Mapper.Map<string, List<Annotation>>(answer.Annotations);
                        cleanedAnnotations.RemoveAll(a => a.docId == docId);
                        answer.Annotations = Mapper.Map<List<Annotation>, string>(cleanedAnnotations);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        throw;
                    }
                }
                return await _unitOfWork.Complete() <= 0 ? StatusCode(500) : NoContent();
            }
            else
            {
                return Ok();
            }
        }
    }
}