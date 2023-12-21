using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using LearnosityDotNetHelper;


namespace LrnGptApi.Controllers;


[ApiController]
[Route("[controller]")]
public class QuestionController : ControllerBase
{
   
   [HttpPost(Name ="PostQuestion")]
   public string Post(GPTQuestion question)
   {
      Question lrnQuestion = new()
      {
         Type = QuestionTypes.Mcq,
         Reference = Guid.NewGuid().ToString()
         
      };

      lrnQuestion.Data.Stimulus=question.Stimulus;
      lrnQuestion.Data.Options.Add(new Option(){Label=question.AnswerChoice1,Value=question.AnswerChoice1});
      lrnQuestion.Data.Options.Add(new Option(){Label=question.AnswerChoice2,Value=question.AnswerChoice2});
      lrnQuestion.Data.Options.Add(new Option(){Label=question.AnswerChoice3,Value=question.AnswerChoice3});
      lrnQuestion.Data.Options.Add(new Option(){Label=question.AnswerChoice4,Value=question.AnswerChoice4});
      lrnQuestion.Data.Type=QuestionTypes.Mcq;
      lrnQuestion.Data.Validation.ScoringType="exactMatch";
      lrnQuestion.Data.Validation.ValidResponse.Score=1;
      lrnQuestion.Data.Validation.ValidResponse.Value.Add(question.CorrectAnswer);
    
Meta meta= new();
meta.User.ID="bradhunt";
meta.User.FirstName="Brad";
meta.User.LastName="Hunt";
meta.User.Email="brad.hunt@learnosity.com";

Questions questions = new();
questions.Meta=meta;
questions.Question.Add(lrnQuestion);

Item item = new Item
{
    Reference = Guid.NewGuid().ToString(),
    Status = "published"
};

      item.QuestionReferences.Add(new QuestionReference(){Reference=lrnQuestion.Reference});
      item.Definition.Widgets.Add(new Widget(){Reference=lrnQuestion.Reference});

      Items items = new();
      items.Meta=meta;
      items.Item.Add(item);

      ItemBank.SetQuestions(questions);
      string status=ItemBank.SetItems(items);

      return status;
   }


}