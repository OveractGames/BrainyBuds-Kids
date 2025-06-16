using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace UnityGame
{
    public class Cap1Level1Logic : MonoBehaviour
    {

        [SerializeField] private TMP_Text _countdown;
        [SerializeField] private float _duration;
        private GameState _currState = GameState.None;
        private float _currTime;

        private void OnEnable()
        {
            if (GameManager.CanStartGame)
            {
                OnReadyToStart();
            }
            else
            {
                GameManager.OnReadyToStartGame += OnReadyToStart;
            }
        }

        private void OnDisable()
        {
            GameManager.OnReadyToStartGame -= OnReadyToStart;
        }

        private void OnReadyToStart()
        {
            Debug.Log($"[Cap1Level1Logic] OnReadyToStart received.");
            GoNextState();
        }

        private void GoNextState()
        {
            switch (_currState)
            {
                case GameState.None:
                    _currTime = _duration;
                    _currState = GameState.Intro;
                    GameManager.Instance.RunIntro();
                    break;
                case GameState.Intro:
                    _currTime = _duration;
                    _currState = GameState.Gameplay;
                    GameManager.Instance.RunGameplay();
                    break;
                case GameState.Gameplay:
                    _currTime = _duration;
                    _currState = Random.Range(0,2) == 1? GameState.OutroGood : GameState.OutroBad;
                    GameManager.Instance.RunOutro(_currState == GameState.OutroGood);
                    break;
                case GameState.OutroGood:
                case GameState.OutroBad:
                    // _currTime = _duration;
                    //  _currState = State.Finished;
                    _currState = GameState.None;
                    GameManager.Instance.FinishGame(null);
                    break;
            }
        }

        private void Update()
        {
            if (_currState == GameState.None)
            {
                return;
            }


            _currTime -= Time.deltaTime;
            _currTime = Math.Max(0, _currTime);
            _countdown.text = $"{_currState} ends in " + ((int)_currTime).ToString();

            if(_currTime == 0)
            {
                GoNextState();
            }
        }
    }
}

