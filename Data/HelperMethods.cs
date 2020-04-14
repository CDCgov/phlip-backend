using System.Collections.Generic;
using System.Threading.Tasks;
using Esquire.Models;

namespace Esquire.Data
{
    public class HelperMethods
    {
        private readonly IUnitOfWork _unitOfWork;
        
        public HelperMethods(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<long>> DeleteSchemeQuestionAndAllNestedQuestions(SchemeQuestionWithChildQuestionsDto question)
        {
            var deletedQuestionIds = new List<long>();
            
            if (question == null) return deletedQuestionIds;
            
            if (question.ChildQuestions != null && question.ChildQuestions.Count != 0)
            {
                foreach (var childQuestion in question.ChildQuestions)
                {
                    deletedQuestionIds.Add(childQuestion.Id);
                    await DeleteSchemeQuestionAndAllNestedQuestions(childQuestion);
                }
            }

            if (await DeleteDataAssociatedWithSchemeQuestion(question.Id))
            {
                deletedQuestionIds.Add(question.Id);
                var schemeQuestion =
                    await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == question.Id);

                _unitOfWork.SchemeQuestions.Remove(schemeQuestion);
            }
            

            return deletedQuestionIds;
        }

        private async Task<bool> DeleteDataAssociatedWithSchemeQuestion(long schemeQuestionId)
        {
            var schemeQuestion = await _unitOfWork.SchemeQuestions.SingleOrDefaultAsync(sq => sq.Id == schemeQuestionId);
            var codedQuestionBases = await 
                _unitOfWork.CodedQuestionBases.FindAsync(cq => cq.SchemeQuestion.Id == schemeQuestionId);
            var schemeQuestionFlags = await _unitOfWork.SchemeQuestionFlags.FindAsync(f => f.SchemeQuestionId == schemeQuestionId);

            foreach (var codedQuestion in codedQuestionBases)
            {
                _unitOfWork.CodedAnswers.RemoveRange(codedQuestion.CodedAnswers);
                
                if (codedQuestion.Flag != null)
                {
                    _unitOfWork.CodedQuestionFlags.Remove(codedQuestion.Flag);
                }
            }
            
            _unitOfWork.SchemeQuestionFlags.RemoveRange(schemeQuestionFlags);
            
            _unitOfWork.SchemeAnswers.RemoveRange(schemeQuestion.PossibleAnswers);
            return true;
        }
    }
}