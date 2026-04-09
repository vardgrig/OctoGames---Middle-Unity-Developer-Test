using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // instead of UnityEngine.UI for TextMeshPro support
// using UnityEngine.UI;

namespace UI
{
    // ???
    // I don't get the idea of this class.
    // It seems to be a UI component that displays the count and average value of a list of Character objects.
    // ???
    public class CharactersView : MonoBehaviour 
    {
        // FIX: [SerializedField] to [SerializeField] typo
        // FIX: Store Character directly instead of Transform to avoid GetComponent every frame
        [SerializeField] private List<Character> _characters = new();

        // FIX: Cache the Text component in Awake, not GetComponent every frame
        private TMP_Text _text;
        //private Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            //_text = GetComponent<Text>();

            if (_text == null)
            {
                Debug.LogError($"[{nameof(CharactersView)}] No Text component found on " +
                               $"'{gameObject.name}'. Attach one or change the reference.");
            }
        }

        // FIX: Subscribe to Character value changes in OnEnable and unsubscribe in OnDisable
        private void OnEnable()
        {
            HandleCharacterValueChangeSubscription(true);
        }
        
        private void OnDisable()
        {
            HandleCharacterValueChangeSubscription(false);
        }
        
        private void HandleCharacterValueChangeSubscription(bool subscribe)
        {
            foreach (var character in _characters)
            {
                if (subscribe)
                {
                    character.ValueChanged += OnCharacterValueChanged;
                }
                else
                {
                    character.ValueChanged -= OnCharacterValueChanged;
                }
            }
        }

        // FIX: FixedUpdate is the physics timestep, which is wrong place for UI.
        // RefreshDisplay should be called via events when character values change, not on a fixed timestep or every frame.
        // Or we can use Update and timestep variable (for example, refresh every 0.5 seconds) if we want to limit the refresh rate.
        private void RefreshDisplay()
        {
            if (!_text)
            {
                return;
            }

            var count = _characters.Count; // FIX: List<T>.Count, not .Length

            if (count == 0) // FIX: Guard against empty list before dividing by zero
            {
                _text.text = "Characters: 0  Avg value: N/A";
                return;
            }

            var totalValue = 0f; // FIX: No GetComponent inside the loop
            for (var i = 0; i < count; i++)
            {
                if (_characters[i])
                {
                    totalValue += _characters[i].Value;
                }
            }

            var average = totalValue / count;  // FIX (logic): "Average = totalValue / count" protected against division by zero

            _text.text = $"Characters: {count}  Avg value: {average:F2}";
            Debug.Log($"CharactersView updated: {_text.text}");
        }
        
        private void OnCharacterValueChanged(float _) => RefreshDisplay();
    }

    // If CharactersView is meant to display various attributes of characters (like Health, Money, etc.)
    // it would be better to have a base class for those attributes that includes a Name and Value property.
    // This way, the CharactersView can be more flexible and display different types of attributes without needing to know the specific implementation details of each attribute type.
    // Also, this way we should rename CharactersView to something like CharacterAttributesView to better reflect its purpose.
    #region CharacterAttribute recommendation
    
    public abstract class CharacterAttribute : MonoBehaviour
    {
        public event Action<CharacterAttribute, float> ValueChanged;

        [SerializeField] private string _name;
        [SerializeField] private float _value;

        public virtual string Name
        {
            get => _name;
            set => _name = value;
        }

        public virtual float Value
        {
            get => _value;
            set
            {
                if (Mathf.Approximately(_value, value)) return;

                _value = value;
                OnValueChanged(_value);
            }
        }

        protected virtual void OnValueChanged(float newValue)
        {
            ValueChanged?.Invoke(this, newValue);
        }
    }

    public class Money : CharacterAttribute
    {
        private void Reset()
        {
            Name = "Money";
            Value = 100f;
        }
    }
    
    public class Health : CharacterAttribute
    {
        private void Reset()
        {
            Name = "Health";
            Value = 10f;
        }

        protected override void OnValueChanged(float newValue)
        {
            base.OnValueChanged(newValue);

            if (newValue <= 0)
            {
                Debug.Log("Character died");
            }
        }
    }

    #endregion
    
    public class Character : MonoBehaviour
    {
        public event Action<float> ValueChanged;

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(_value);
            }
        }
    }
}