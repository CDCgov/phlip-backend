using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using esquire;
using Esquire.Data;
using Esquire.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace esquire_backend.Controllers
{
    [Produces("application/json")]
    [Route("api/projects/{projectId}/scheme")]
    [Authorize]
    public class SchemeController : Controller
    {
        
        private readonly IUnitOfWork _unitOfWork;

        public SchemeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Get Scheme as Tree for Project 
        /// </summary>
        /// <remarks>Returns a list of SchemeQuestions in a heirarchy based on the outline.</remarks>
        /// <param name="projectId">The id of the Project</param>
        [HttpGet("tree")]
        public async Task<IActionResult> GetTree(long projectId)
        {
            var project = await _unitOfWork.Projects.GetProjectScheme(p => p.Id == projectId);

            if (project?.Scheme?.QuestionNumbering == null || project.Scheme.Questions?.Count == 0)
            {
                return NotFound();
            }
            
            var returnObject = _unitOfWork.Schemes.GetSchemeQuestionsAsTree(project.Scheme);

            return Ok(Mapper.Map<List<SchemeQuestionWithChildQuestionsDto>>(returnObject));
        }

        /// <summary>
        /// Get Scheme for Project
        /// </summary>
        /// <remarks>Returns the Scheme for a project. This includes the list of questions and the outline.</remarks>
        /// <param name="projectId">The id of the Project</param>
        [HttpGet]
        public async Task<IActionResult> GetAll(long projectId)
        {
            var project = await _unitOfWork.Projects.GetProjectScheme(p => p.Id == projectId);

            if (project == null)
            {
                return NotFound();
            }

            var outlineToReturn = project.Scheme.QuestionNumbering != null ? 
                JsonConvert.DeserializeObject(project.Scheme.QuestionNumbering) : 
                new {};

            var returnObject = new SchemeReturnDto
            {
                SchemeQuestions = Mapper.Map<ICollection<SchemeQuestionReturnDto>>(project.Scheme.Questions),
                Outline = outlineToReturn
            };
            return Ok(returnObject);
        }
        
        /// <summary>
        /// Retrieve a scheme question by using a scheme question ID
        /// </summary>
        /// <remarks>Returns the specified scheme question.</remarks>
        /// <param name="projectId">The id of the Project</param>
        /// <param name="schemeQuestionId">The id of the SchemeQuestion</param>
        [HttpGet("{schemeQuestionId}", Name = "GetSchemeQuestionById")]
        public async Task<IActionResult> GetById(long projectId, long schemeQuestionId)
        {
            var question = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            if (question == null)
            {
                return NotFound();
            }

            return Ok(Mapper.Map<SchemeQuestionReturnDto>(question));
        }

        /// <summary>
        /// Create a scheme question
        /// </summary>
        /// <remarks>
        /// Creates a new scheme question. 
        /// </remarks>
        /// <param name="projectId">The id of the Project</param>
        /// <param name="questionToCreate">The question object to create.</param>
        [HttpPost]
        public async Task<IActionResult> Create(long projectId, [FromBody] SchemeQuestionCreationDto questionToCreate)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == questionToCreate.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user)) return Forbid();

            var project = await _unitOfWork.Projects.GetProjectSchemeWithoutFlags(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }

            if (project.Status != (int) ProjectStatus.Active)
            {
                return Forbid();
            }

            var question = Mapper.Map<SchemeQuestion>(questionToCreate);
            var scheme = project.Scheme;
            scheme.Questions.Add(question);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            var outline = questionToCreate.Outline;
            
            outline.Add(question.Id,
                new OrderInfo
                {
                    parentId = questionToCreate.ParentId,
                    positionInParent = questionToCreate.PositionInParent
                });
            
            scheme.QuestionNumbering = JsonConvert.SerializeObject(outline);

            project.UpdateLastEditedDetails(user);
            
            await _unitOfWork.Complete();

            return CreatedAtRoute("GetSchemeQuestionById", new {schemeQuestionId = question.Id}, Mapper.Map<SchemeQuestionReturnDto>(question));
        }
        
        /// <summary>
        /// Update the scheme outline
        /// </summary>
        /// <remarks>
        /// Stores a new outline, the question ordering, for a scheme. This is used by the UI to modify the order/heirarchy of questions in the scheme.  
        /// </remarks>
        /// <param name="projectId">The id of the Project</param>
        /// <param name="orderUpdateDto">The SchemeOrderUpdate object</param>
        [HttpPut]
        public async Task<IActionResult> UpdateOrder(long projectId, [FromBody] SchemeOrderUpdateDto orderUpdateDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == orderUpdateDto.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user)) return Forbid();

            var project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }
            if (project.Status != (int) ProjectStatus.Active)
            {   // project locked no update allowed
                return Forbid();  
            }
            var scheme = project.Scheme;
            scheme.QuestionNumbering = JsonConvert.SerializeObject(orderUpdateDto.Outline);

            project.UpdateLastEditedDetails(user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return NoContent();
        }

        /// <summary>
        /// Update a scheme question
        /// </summary>
        /// <remarks>
        /// Update the specified scheme question. This is used to update any part of a scheme question,
        /// including the list of scheme answers.
        /// </remarks>
        /// <param name="projectId">The id of the project</param>
        /// <param name="questionId">The id of the question to update.</param>
        /// <param name="questionUpdates">The updated question object</param>
        [HttpPut("{questionId}")]
        public async Task<IActionResult> UpdateQuestion(long projectId, long questionId, [FromBody] SchemeQuestionUpdateDto questionUpdates)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == questionUpdates.UserId);
            if (!_unitOfWork.Users.CanUserEdit(user)) return Forbid();
            
            var project = await _unitOfWork.Projects.SingleOrDefaultAsync(p => p.Id == projectId);
            if (project.Status != (int) ProjectStatus.Active)
            {   
                // project locked no update allowed
                return StatusCode(403);  
            }
            if (project == null)
            {
                return NotFound();
            }
            
            var questionToUpdate = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == questionId);
            if (questionToUpdate == null)
            {
                return NotFound();
            }
            
            Mapper.Map(questionUpdates, questionToUpdate);

            UpdateAnswersForQuestion(questionToUpdate, questionUpdates);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            project.UpdateLastEditedDetails(user);

            if (await _unitOfWork.Complete() <= 0)
            {
                return StatusCode(304);
            }

            return Ok(Mapper.Map<SchemeQuestionReturnDto>(questionToUpdate));

        }

        /// <summary>
        /// Delete a scheme question
        /// </summary>
        /// <remarks>
        /// Delete a specific scheme question using scheme question id. This requires the deletion of related data. Request
        /// fails if the requestor is not an admin or coordinator.
        /// </remarks>
        /// <param name="projectId">The id of the Project</param>
        /// <param name="schemeQuestionId">The id of the scheme question to be deleted.</param>
        [HttpDelete("{schemeQuestionId}")]
        public async Task<IActionResult> DeleteQuestion(long projectId, long schemeQuestionId)
        {
            // check if the requestor is an admin or coordinator. 
            int.TryParse(HttpContext.User.Claims.FirstOrDefault(c => c.Type.Equals("Id"))?.Value, out var requestorId);
            var user = await _unitOfWork.Users.SingleOrDefaultAsync(u => u.Id == requestorId);
            if (!_unitOfWork.Users.CanUserEdit(user))
            {
                return Forbid();
            }

            var project = await _unitOfWork.Projects.GetProjectWithScheme(p => p.Id == projectId);
            if (project == null)
            {
                return NotFound();
            }
            if (project.Status != (int) ProjectStatus.Active)
            {   // project locked no update allowed
                return StatusCode(403);  
            }
            var schemeQuestionTree = _unitOfWork.Schemes.GetSchemeQuestionsAsTree(project.Scheme);
            if (schemeQuestionTree == null || schemeQuestionTree.Count == 0) return StatusCode(304);
            
            var questionToDelete = _unitOfWork.SchemeQuestions.FindSchemeQuestionWithChildQuestionsDto(schemeQuestionId, schemeQuestionTree);

            if (questionToDelete == null) return NotFound();
            
            var helperMethods = new HelperMethods(_unitOfWork);

            var deletedQuestionIds = await helperMethods.DeleteSchemeQuestionAndAllNestedQuestions(questionToDelete);

            var outline = JsonConvert.DeserializeObject<Dictionary<long, OrderInfo>>(project.Scheme.QuestionNumbering);

            foreach (var id in deletedQuestionIds)
            {
                outline.Remove(id);
            }
            
            project.Scheme.QuestionNumbering = JsonConvert.SerializeObject(outline);

            await _unitOfWork.Complete();
            
            var returnObject = new SchemeReturnDto
            {
                SchemeQuestions = Mapper.Map<ICollection<SchemeQuestionReturnDto>>(project.Scheme.Questions),
                Outline = JsonConvert.DeserializeObject(project.Scheme.QuestionNumbering)
            };

            return Ok(returnObject);
        }

        /// <summary>
        /// Update scheme answers for a scheme question
        /// </summary>
        /// <remarks>
        /// This is the logic used to update answers for an existing scheme question.
        /// check old answer array against new answer array
        /// if an existing answer is NOT in the new array, delete the exiting answer
        /// if existing answer has new text, update existing answer
        /// add any new answers
        /// </remarks>
        /// <param name="questionToUpdate"></param>
        /// <param name="questionUpdates"></param>
        private void UpdateAnswersForQuestion(SchemeQuestion questionToUpdate,
            SchemeQuestionUpdateDto questionUpdates)
        {
            foreach (var oldAnswer in questionToUpdate.PossibleAnswers)
            {
                var newAnswerDto = questionUpdates.PossibleAnswers.SingleOrDefault(a => a.Id == oldAnswer.Id);

                if (newAnswerDto != null)
                {
                    if (oldAnswer.Text != newAnswerDto.Text)
                    {
                        oldAnswer.Text = newAnswerDto.Text;
                    }
                    if (oldAnswer.Order != newAnswerDto.Order)
                    {
                        oldAnswer.Order = (int) newAnswerDto.Order;
                    }
                }
                else
                {
                    // This will fail if there are CodedAnswers for this SchemeAnswer.
                    // If we want to support deleting answers, this needs to be updated.
                    _unitOfWork.SchemeAnswers.Remove(oldAnswer);
                }
            }

            // Look for new answers and add them.
            // New answers have no Id so check for Id == null.
            foreach (var answer in questionUpdates.PossibleAnswers)
            {
                if (answer.Id == null)
                {
                    var answerToAdd = Mapper.Map<SchemeAnswer>(answer);
                    questionToUpdate.PossibleAnswers.Add(answerToAdd);
                }
            }
        }
    }
}