using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class Health_Pattern_Manager : MonoBehaviour
{
    [Serializable]
    public struct Threshold
    {
        [Range(0, 100)]
        public float AvailableAtHealthPercent;
        public List<int> PatternsIndex;
    }

    [SerializeField]
    public List<Threshold> PatternThresholds;
    private PatternHandler _patternHandler;
    private Health _health;
    private Queue<float> _currentPatternsHealthValue;

    private void Start()
    {
        _patternHandler = GetComponent<PatternHandler>();
        _health = GetComponent<Health>(); 
        _health.ChangeInHealth += SetPatternsByHealth;
        _currentPatternsHealthValue = new Queue<float>();
        OrderPatternsForQueue();
        SetPatternsByHealth(_health.StartingHealthValue);
    }

    /// <summary>
    /// Assumes all available patterns are only in 1 Threshold at certain health percentage.
    /// As such, if multiple Thresholds are set to the same percentage, only the fist result will be used.
    /// </summary>
    /// <param name="Health"></param>
    void SetPatternsByHealth(float Health)
    {
        var hp = ConvertHPtoPercentage(Health);
        if (_currentPatternsHealthValue.Count == 0) {
            return;
        }

        var peek = _currentPatternsHealthValue.Peek();
        if (_currentPatternsHealthValue.Count > 0 && hp <= peek) {
            var patterns = PatternThresholds.First(x => x.AvailableAtHealthPercent == peek).PatternsIndex;
            _patternHandler.SetPatternByIDs(patterns);
            _currentPatternsHealthValue.Dequeue();
        }
    }

    float ConvertHPtoPercentage(float health)
    {
        if (_health.MaxHealthValue == 0) {
            return 0;
        }

        return (health / _health.MaxHealthValue * 100);
    }

    /// <summary>
    /// Orders patterns based on highest health Thresholds at the front of the queue.
    /// </summary>
    void OrderPatternsForQueue()
    {
        var temp = PatternThresholds;
        var order = temp.OrderByDescending(x => x.AvailableAtHealthPercent).ToList();

        foreach (Threshold t in order) {
            _currentPatternsHealthValue.Enqueue(t.AvailableAtHealthPercent);
        }
    }
}
