using System.Collections.Generic;

namespace Esquire.Services
{
    public class ExportResult
    {
        public string Message { get; set; }
        public int Code { get; set; }
        public string FilePath { get; set; }

        public const int Success = 1;
        public const int NoError = 0;
        public const int ProjectNotFound = -1;
        public const int SchemeNotFound = -2;
        public const int CategoryHeaderInvalidOperation = -3;
        public const int CategoryHeaderArgumentNull = -4;
        public const int CheckboxHeaderInvalidOperation = -5;
        public const int CheckboxHeaderArgumentNull = -6;
        public const int HeaderInvalidQuestionType = -7;
        public const int AnswerInvalidQuestionType = -8;
        public const int BinaryAnswerInvalidOperation = -9;
        public const int BinaryAnswerArgumentNull = -10;
        public const int CheckboxAnswerInvalidOperation = -11;
        public const int CheckboxAnswerArgumentNull = -12;
        public const int MultipleChoiceAnswerInvalidOperation = -13;
        public const int MultipleChoiceAnswerArgumentNull = -14;
        public const int TextAnswerInvalidOperation = -15;
        public const int TextAnswerArgumentNull = -16;

       
        private readonly Dictionary<int,string> ExportResultsCodes = new Dictionary<int,string>()
        {
            { Success, "The project was exported without any errors."},
            { NoError, "The export result error code was not set. An unknown error or exception may have occurred."},
            { ProjectNotFound, "Unable to retrieve the project from the database." },
            { SchemeNotFound, "Unable to retrieve the project scheme ordered by outline."},
            { CategoryHeaderInvalidOperation, "Invalid Operation Exception thrown while writing category question header, looking for a single scheme answer with a matching order integer."},
            { CategoryHeaderArgumentNull, "Null Reference Exception thrown while writing category question header, looking for a single scheme answer with a matching order integer."},
            { CheckboxHeaderInvalidOperation, "Invalid Operation Exception thrown while writing checkbox question header, looking for a single scheme answer with a matching order integer."},
            { CheckboxHeaderArgumentNull, "Null Reference Exception thrown while writing checkbox question header, looking for a single scheme answer with a matching order integer."},
            { HeaderInvalidQuestionType, "Invalid question type encountered while writing headers."},
            { AnswerInvalidQuestionType, "Invalid question type encountered while writing answers."},
            { BinaryAnswerInvalidOperation, "Invalid Operation Exception thrown while writing binary answer field."},
            { BinaryAnswerArgumentNull, "Null Reference Exception thrown while writing binary answer field."},
            { CheckboxAnswerInvalidOperation, "Invalid Operation Exception thrown while writing checkbox answer field."},
            { CheckboxAnswerArgumentNull, "Null Reference Exception thrown while writing checkbox answer field."},
            { MultipleChoiceAnswerInvalidOperation, "Invalid Operation Exception thrown while writing multiple choice answer field."},
            { MultipleChoiceAnswerArgumentNull, "Null Reference Exception thrown while writing multiple choice answer field."},
            { TextAnswerInvalidOperation, "Invalid Operation Exception thrown while writing text answer field."},
            { TextAnswerArgumentNull, "Null Reference Exception thrown while writing text answer field."},

        };

        public ExportResult(string filePath)
        {
            this.Code = Success;
            this.Message = ExportResultsCodes[this.Code];
            this.FilePath = filePath;
        }
     
        public ExportResult(int code)
        {
            this.Code = code;
            this.Message = ExportResultsCodes[code];
            this.FilePath = null;
        }

        public ExportResult()
        {
            this.Code = NoError;
            this.Message = ExportResultsCodes[this.Code];
        }

        public bool IsSuccessful()
        {
            if (this.Code == Success)
                return true;
            else
                return false;
        }

        public bool HasError()
        {
            if (this.Code < 0)
                return true;
            else
                return false;
        }

        public void SetError(int code)
        {
            this.Code = code;
            this.Message = ExportResultsCodes[code];
            this.FilePath = null;
            
        }


       
    }
}