using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecisionTree : MonoBehaviour
{
    #region AI DECLARATIONS
    INode _rootAI;

    ActionNode _buildAction;
    ActionNode _cutAction;
    ActionNode _dieAction;
    ActionNode _eatAction;
    ActionNode _findPartnerAction;
    ActionNode _harvestAction;
    ActionNode _lookForFamilyAction;
    ActionNode _playAction;
    ActionNode _restAction;

    QuestionNode _hasStaminaQuestion;
    QuestionNode _theresBuildingQuestion;
    QuestionNode _theresFoodQuestion;
    QuestionNode _theresFoodStockQuestion;
    QuestionNode _theresWoodQuestion;
    QuestionNode _whatGenderQuestion;
    QuestionNode _whatStatusQuestion;
    QuestionNode _whatTimeAdultsQuestion;
    QuestionNode _whatTimeKidsQuestion;
    QuestionNode _whatTimeOldsQuestion;
    QuestionNode _whatWeatherAdultsQuestion;
    QuestionNode _whatWeatherKidsQuestion;
    QuestionNode _whatWeatherOldsQuestion;

    OptionsNode _hasLifeOptions;
    List<INode> _hasLifeList;
    OptionsNode _howOldOptions;
    List<INode> _howOldList;
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
        _buildAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _cutAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _dieAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _eatAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _findPartnerAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _harvestAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _lookForFamilyAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _playAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        _restAction = new ActionNode(() =>
        {
            //DO SOMETHING
        });
        #endregion

        #region QUESTIONS
        _theresWoodQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _buildAction, _cutAction);
        _whatStatusQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _lookForFamilyAction, _findPartnerAction);
        _theresFoodStockQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatStatusQuestion, _harvestAction);
        _whatGenderQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _theresWoodQuestion, _theresFoodStockQuestion);
        _whatWeatherAdultsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatGenderQuestion, _whatStatusQuestion);
        _whatTimeAdultsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherAdultsQuestion, _restAction);
        _whatWeatherKidsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _playAction, _restAction);
        _whatTimeKidsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherKidsQuestion, _restAction);
        _theresBuildingQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _buildAction, _restAction);
        _whatWeatherOldsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _theresBuildingQuestion, _restAction);
        _whatTimeOldsQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _whatWeatherOldsQuestion, _restAction);
        _howOldList = new List<INode>();
        _howOldList.Add(_whatTimeKidsQuestion);
        _howOldList.Add(_whatTimeAdultsQuestion);
        _howOldList.Add(_whatTimeOldsQuestion);
        _howOldOptions = new OptionsNode(() =>
        {
            return 0; //COMPARE SOMETHING
        }, _howOldList);
        _hasStaminaQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _howOldOptions, _restAction);
        _theresFoodQuestion = new QuestionNode(() =>
        {
            return true; //COMPARE SOMETHING
        }, _eatAction, _harvestAction);
        _hasLifeList = new List<INode>();
        _hasLifeList.Add(_dieAction);
        _hasLifeList.Add(_theresFoodQuestion);
        _hasLifeList.Add(_hasStaminaQuestion);
        _hasLifeOptions = new OptionsNode(() =>
        {
            return 0; //COMPARE SOMETHING
        }, _hasLifeList);
        #endregion

        _rootAI = _hasLifeOptions;
    }
}
