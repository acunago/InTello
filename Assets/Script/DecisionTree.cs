using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    #region AI DECLARATIONS
    IDecision _rootAI;

    ActionDT _buildAction;
    ActionDT _cutAction;
    ActionDT _dieAction;
    ActionDT _eatAction;
    ActionDT _findPartnerAction;
    ActionDT _harvestAction;
    ActionDT _lookForFamilyAction;
    ActionDT _playAction;
    ActionDT _restAction;

    QuestionDT _hasStaminaQuestion;
    QuestionDT _theresBuildingQuestion;
    QuestionDT _theresFoodQuestion;
    QuestionDT _theresFoodStockQuestion;
    QuestionDT _theresWoodQuestion;
    QuestionDT _whatGenderQuestion;
    QuestionDT _whatStatusQuestion;
    QuestionDT _whatTimeAdultsQuestion;
    QuestionDT _whatTimeKidsQuestion;
    QuestionDT _whatTimeOldsQuestion;
    QuestionDT _whatWeatherAdultsQuestion;
    QuestionDT _whatWeatherKidsQuestion;
    QuestionDT _whatWeatherOldsQuestion;

    OptionsDT _hasLifeOptions;
    List<IDecision> _hasLifeList;
    OptionsDT _howOldOptions;
    List<IDecision> _howOldList;
    #endregion

    void Start()
    {
        GenerateMyAI();
    }

    void Update()
    {
        if (_rootAI != null) _rootAI.Execute();
    }

    /// <summary>
    /// Genera la IA del citizen.
    /// </summary>
    void GenerateMyAI()
    {
        #region ACTIONS
        _buildAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _cutAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _dieAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _eatAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _findPartnerAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _harvestAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _lookForFamilyAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _playAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        _restAction = new ActionDT(() =>
        {
            //DO SOMETHING
        });
        #endregion

        #region QUESTIONS
        _theresWoodQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _buildAction, _cutAction);
        _whatStatusQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _lookForFamilyAction, _findPartnerAction);
        _theresFoodStockQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatStatusQuestion, _harvestAction);
        _whatGenderQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _theresWoodQuestion, _theresFoodStockQuestion);
        _whatWeatherAdultsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatGenderQuestion, _whatStatusQuestion);
        _whatTimeAdultsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherAdultsQuestion, _restAction);
        _whatWeatherKidsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _playAction, _restAction);
        _whatTimeKidsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherKidsQuestion, _restAction);
        _theresBuildingQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _buildAction, _restAction);
        _whatWeatherOldsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _theresBuildingQuestion, _restAction);
        _whatTimeOldsQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherOldsQuestion, _restAction);
        _howOldList = new List<IDecision>();
        _howOldList.Add(_whatTimeKidsQuestion);
        _howOldList.Add(_whatTimeAdultsQuestion);
        _howOldList.Add(_whatTimeOldsQuestion);
        _howOldOptions = new OptionsDT(() =>
        {
            return 0; //COMPARE SOMETHING
        }, _howOldList);
        _hasStaminaQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _howOldOptions, _restAction);
        _theresFoodQuestion = new QuestionDT(() =>
        {
            return true; //COMPARE SOMETHING
        }, _eatAction, _harvestAction);
        _hasLifeList = new List<IDecision>();
        _hasLifeList.Add(_dieAction);
        _hasLifeList.Add(_theresFoodQuestion);
        _hasLifeList.Add(_hasStaminaQuestion);
        _hasLifeOptions = new OptionsDT(() =>
        {
            return 0; //COMPARE SOMETHING
        }, _hasLifeList);
        #endregion

        _rootAI = _hasLifeOptions;
    }
}
