using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoberiaHpSlider : MonoBehaviour
{

    [Header("Set RoberiaAsteroid Prefab")]
    //隕石プレハブ
    public GameObject _roberiaPrefab;

    Slider _slider;
    private float _hp = 0;
    private bool _hpsetflg = true;

    void Start()
    {
        // スライダーを取得する
        _slider = this.gameObject.GetComponent<Slider>();
        _slider.maxValue = _roberiaPrefab.GetComponent<AsteroidScript>().GetAsteroidHp();
    }

    
    void Update()
    {
        if (_hpsetflg)
        {
            Hpset();
        }
        else
        {
            _slider.value = _roberiaPrefab.GetComponent<AsteroidScript>().GetAsteroidHp();
            
        }
        if (_roberiaPrefab.GetComponent<AsteroidScript>().GetAsteroidHp() <= 10)
        {
            Destroy(this.gameObject);
        }
    }

    void Hpset()
    {
        // HP上昇
        _hp += 10f;
        if (_hp > _slider.maxValue)
        {
            // 最大を超えたら0に戻す
            _hp = _slider.maxValue;
            _hpsetflg = false;
        }

        // HPゲージに値を設定
        _slider.value = _hp;
    }
}
