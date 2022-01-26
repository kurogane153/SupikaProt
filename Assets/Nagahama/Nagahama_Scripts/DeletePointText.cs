using UnityEngine;
using TMPro;

public class DeletePointText : MonoBehaviour
{
    private RectTransform rectTransform;

    /// <summary>
    /// 得点テキストオブジェクトのパラメータ設定
    /// </summary>
    /// <param name="text"></param>
    /// <param name="pos"></param>
    /// <param name="isFall"></param>
    public void SetDeletePointText(string text, Vector3 pos, bool addRandomPos = true)
    {
        GetComponent<TextMeshProUGUI>().text = text;
        rectTransform = GetComponent<RectTransform>();

        // Canvasの子になる
        transform.SetParent(ScoreManager.Instance.GetCanvas().transform);

        // 引数の位置に移動
        transform.position = pos;

        transform.position = new Vector3(transform.position.x, transform.position.y, 0);

        // ローカルScaleを1,1,1にする
        transform.localScale = Vector3.one;

        // 画面からみ出た場合、画面内におさまるように位置を調整
        if (rectTransform.position.y > 980f) {
            rectTransform.position = new Vector3(rectTransform.position.x, 980f, 0);
        }
        else if (rectTransform.position.y < 100f) {
            rectTransform.position = new Vector3(rectTransform.position.x, 100f, 0);
        }

        if(rectTransform.position.x > 1820f) {
            rectTransform.position = new Vector3(1820f, rectTransform.position.y, 0);
        }
        else if (rectTransform.position.x < 100f) {
            rectTransform.position = new Vector3(100f, rectTransform.position.y, 0);
        }

        // 位置を多少ランダムにずらす
        if (addRandomPos) {
            Vector3 randPos = new Vector3(Random.Range(-50f, 50f), Random.Range(-100f, 100f), 0);
            rectTransform.position += randPos;
        }

        Destroy(gameObject, 3f);
    }

}