using System.Collections.Generic;
using UnityEngine;

public class BossScenePlayer : MonoBehaviour
{
    public List<GameObject> groundPrefabs;
    //public PlayerController player;

    private float _groundWidth;
    private float _totalGroundWidth;
    private List<GameObject> _groundInstances;

    private void Awake()
    {
        _groundInstances = new List<GameObject>();
        foreach (GameObject groundInstance in groundPrefabs)
        {
            GameObject ground = Instantiate(groundInstance, transform);
            ground.transform.position = new Vector3(_totalGroundWidth, 0, 0);
            // ���⼭ array ���� ��Ż ���̸� ���
            _totalGroundWidth += ground.transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
            _groundInstances.Add(ground);
        }

        // ù��° array�� �ִ� element �� ���̸� ����
        _groundWidth = _groundInstances[0].transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
    }

    private void Update()
    {
        // ����
        foreach (GameObject ground in _groundInstances)
        {
            //ground.transform.position -= new Vector3(player.speed * Time.deltaTime, 0, 0);
        }

        // ���� array���� ù��° element�� x ��ǥ�� -_groundWidth ���� �۰ų� ���ٸ�
        if (_groundInstances[0].transform.position.x <= -_groundWidth)
        {
            // ù��° element�� ��ǥ�� ��Ż ���̸� �����Ͽ� ����
            _groundInstances[0].transform.position = new Vector3
            (
                _totalGroundWidth - _groundInstances[0].transform.position.x,
                _groundInstances[0].transform.position.y,
                _groundInstances[0].transform.position.z
            );
            // ù��° element�� array�� ������ element�� �ű�
            GameObject obj = _groundInstances[0];
            _groundInstances.RemoveAt(0);
            _groundInstances.Add(obj);
            // 0��° element ���� ������
            _groundWidth = _groundInstances[0].transform.Find("Ground").GetComponent<SpriteRenderer>().bounds.size.x;
        }
    }
}
