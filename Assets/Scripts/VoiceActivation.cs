/*
 * Copyright (c) Meta Platforms, Inc. and affiliates.
 * All rights reserved.
 *
 * Licensed under the Oculus SDK License Agreement (the "License");
 * you may not use the Oculus SDK except in compliance with the License,
 * which is provided at the time of installation or download, or which
 * otherwise accompanies this software in either electronic or hard copy form.
 *
 * You may obtain a copy of the License at
 *
 * https://developer.oculus.com/licenses/oculussdk/
 *
 * Unless required by applicable law or agreed to in writing, the Oculus SDK
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Meta.WitAi;
using Meta.WitAi.Requests;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Oculus.VoiceSDK.UX
{
    [RequireComponent(typeof(Button))]
    public class VoiceActivationButton : MonoBehaviour
    {
        // The button to be observed
        private Button _button;
        // The button label to be adjusted with state
        private TMP_Text _buttonLabel;

        [Tooltip("Reference to the current voice service")]
        [SerializeField] private VoiceService _voiceService;

        [Tooltip("Text to be shown while the voice service is not active")]
        [SerializeField] private string _activateText = "Activate";
        [Tooltip("Whether to immediately send data to service or to wait for the audio threshold")]
        [SerializeField] private bool _activateImmediately = false;

        [Tooltip("Text to be shown while the voice service is active")]
        [SerializeField] private string _deactivateText = "Deactivate";
        [Tooltip("Whether to immediately abort request activation on deactivate")]
        [SerializeField] private bool _deactivateAndAbort = false;

        // Current request
        private VoiceServiceRequest _request;
        private bool _isActive = false;

        // Get button & label
        private void Awake()
        {
            print("THIS WOWKEE");
            OnClickToggle();
            print("This finished");
            _buttonLabel = GetComponentInChildren<TMP_Text>();
            if (_voiceService == null)
            {
                _voiceService = FindObjectOfType<VoiceService>();
            }
        }
        // Add click delegate
        private void OnEnable()
        {
            RefreshActive();
        }
        // Remove click delegate
        private void OnDisable()
        {
            _isActive = false;
        }

        // On click, activate if not active & deactivate if active
        public void OnClickToggle()
        {
            print("Hi I got here");
            if (!_isActive)
            {
                Activate();
            }
            else
            {
                Deactivate();
            }
        }

        // Activate depending on settings
        private void Activate()
        {

            if (!_activateImmediately)
            {
                _request = _voiceService.Activate(GetRequestEvents());
            }
            else
            {
                _request = _voiceService.ActivateImmediately(GetRequestEvents());
            }
        }

        // Deactivate depending on settings
        private void Deactivate()
        {
            if (!_deactivateAndAbort)
            {
                _request.DeactivateAudio();
            }
            else
            {
                _request.Cancel();
            }
        }

        // Get events
        private VoiceServiceRequestEvents GetRequestEvents()
        {
            VoiceServiceRequestEvents events = new VoiceServiceRequestEvents();
            events.OnInit.AddListener(OnInit);
            events.OnComplete.AddListener(OnComplete);
            return events;
        }
        // Request initialized
        private void OnInit(VoiceServiceRequest request)
        {
            _isActive = true;
            RefreshActive();
        }
        // Request completed
        private void OnComplete(VoiceServiceRequest request)
        {
            _isActive = false;
            RefreshActive();
        }

        // Refresh active text
        private void RefreshActive()
        {
            if (_buttonLabel != null)
            {
                _buttonLabel.text = _isActive ? _deactivateText : _activateText;
            }
        }
    }
}