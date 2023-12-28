
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
      
      Meta meta= new();
      meta.User.ID="bradhunt";
      meta.User.FirstName="Brad";
      meta.User.LastName="Hunt";
      meta.User.Email="brad.hunt@learnosity.com";

      Feature feature = new();
      
      if(!string.IsNullOrWhiteSpace(question.Passage))
      {
         //create a feature
         
         feature.Reference = Guid.NewGuid().ToString();
         feature.Type = "sharedpassage";
         //feature.Data.Heading = "Lorem Ipsum 2";
         feature.Data.Content =question.Passage;
         feature.Data.Type = "sharedpassage";

         //create a list of features, add feature to it
         Features features = new()
         {
            Meta = meta
         };
         features.Feature.Add(feature);

         //submit the list of features to the API
         ItemBank.SetFeatures(features);
      }


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



        Questions questions = new()
        {
            Meta = meta
        };
        questions.Question.Add(lrnQuestion);

      Item item = new()
      {
         Reference = Guid.NewGuid().ToString(),
         Status = "published"
      };

      if(!string.IsNullOrWhiteSpace(question.Passage))
      {
         //add the feature to the item
         item.FeatureReferences.Add(new FeatureReference() { Reference = feature.Reference });
         Widget fWidget=new ();
         fWidget.Reference=feature.Reference;
         List<Widget> fWidgets =new();
         fWidgets.Add(fWidget);
         item.Definition.Regions.Add(new Region(){Type="column",Width=50,Widgets=fWidgets});


         //add the question to the item
         item.QuestionReferences.Add(new QuestionReference(){Reference=lrnQuestion.Reference});
         Widget qWidget=new ();
         qWidget.Reference=lrnQuestion.Reference;
         List<Widget> qWidgets =new();
         qWidgets.Add(qWidget);
         item.Definition.Regions.Add(new Region(){Type="column",Width=50,Widgets=qWidgets});
      }
      else{
         item.QuestionReferences.Add(new QuestionReference(){Reference=lrnQuestion.Reference});
         item.Definition.Widgets.Add(new Widget(){Reference=lrnQuestion.Reference});
      }
     



      Items items = new();
      items.Meta=meta;
      items.Item.Add(item);

      ItemBank.SetQuestions(questions);
      string status=ItemBank.SetItems(items);

      return status;
   }


}