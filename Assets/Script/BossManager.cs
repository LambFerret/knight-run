using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Transform mainCam; //����ķ
    public Transform cam2; //ķ
    public GameObject cam2UI;

    private bool shouldCameraFollow = true; // ī�޶� �÷��̾ ������ ����

    public int curPlayerCount; //���� �÷��̾� ī��Ʈ
    public int curBossCount; //���� ���� ī��Ʈ

    public Transform playerLocation;
    public Transform bossLocation;
    public Transform curPlayerLocation;
    public Transform curPlayerLocation2;
    public Transform curBossLocation;
    public Transform curBossLocation2;

    public Transform cam2Open;
    public Transform cam2Close;

    public GameObject player;
    public GameObject boss;

    public GameObject[] playerAll;
    public GameObject[] bossAll;

    public GameObject rhythmCircle1;
    public GameObject rhythmCircle2;

    public GameObject currentPlayer;
    public GameObject currentBoss;

    public bool isBattleongoing; //��Ʋ�� ���۵Ǿ��°�?
    public bool isstop; //��Ʋ�� ���۵Ǿ��°�?
    bool isFeverTime = false;

    private Vector3 originalSize; // rhythmCircle1�� ���� ũ�⸦ ����

    public int currentPlayerIndex = 0;
    public int currentBossIndex = 0;

    float curTouchTime;

    public enum State
    {
        None, Fever, Bad, Good
    }

    bool turnDone;

    public State state;

    public Vector2 bossDummyLocation;

    //UI
    public GameObject feverUI;

    void Start()
    {
        feverUI.SetActive(false);
        curPlayerCount = 10;
        curBossCount = 50;
        playerAll = new GameObject[curPlayerCount];
        bossAll = new GameObject[curBossCount];
        CreatePlayerNBoss();
        //StartRhythmGame();
        isBattleongoing = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (isBattleongoing)
        {
            StartBattle();
            isBattleongoing = false;
            isstop = false;
        }
        CameraFollow();
        
        if (isFeverTime && Input.GetMouseButtonDown(0) && curTouchTime > 0.1f) //�ǹ�Ÿ��
        {
            
            curBossCount--;
            Debug.Log("bossDummyLocation: " + bossDummyLocation);
            Vector3 destination = bossLocation.transform.position; //���� ��ġ
            int b = Random.Range(7, 20);
            bossDummyLocation = new Vector2(currentPlayer.transform.position.x + 2, currentPlayer.transform.position.y);
            destination.x += 10; //�ڷ� ���ư��� ��ġ. ���� ���� ����
            currentBoss.transform.DOJump(destination, b, 1, 3f); //���������� ���ư���. ù ��° �Ķ���ʹ� ������, �� ��°�� ���� ����, �� ��°�� ���� Ƚ��, �� ��°�� ��ü ���� �ð�
            StartCoroutine(DeactivateBoss());
            if (curBossCount > 0)
            {
                currentBossIndex++;
                currentBoss = bossAll[currentBossIndex];
            }
            currentBoss.transform.DOMove(bossDummyLocation, 0.07f);
            Debug.Log("Boss count: " + curBossCount);
            curTouchTime = 0;
        }

        if(curTouchTime <= 0.2)
        {
            curTouchTime += Time.deltaTime;
        }


    }
    
    void CameraFollow()
    {
        // Check if currentBoss is close to currentPlayer
        float distance = (currentPlayer.transform.position - currentBoss.transform.position).magnitude;

        if (distance < 10.0f)  // you can adjust this value
        {
            // If the currentBoss is close, stop the camera
            shouldCameraFollow = false;
            if (!isstop)
            {
                isstop = true;
                Invoke("PlayerBossClose", 0.1f);
            }

        }

        if (shouldCameraFollow)
        {
            // Move the camera to follow currentPlayer
            mainCam.position = currentPlayer.transform.position + new Vector3(5, 2, -10);  // adjust the offset if needed
            cam2.position = currentBoss.transform.position + new Vector3(8, 2.5f, -10);  // adjust the offset if needed
        }
    }

    void PlayerBossClose()
    {
        currentPlayer.transform.DOKill();
        currentBoss.transform.DOKill();
        currentPlayer.transform.DOMove(bossLocation.position, 40);
        currentBoss.transform.DOMove(playerLocation.position, 40);
    }

    void StartRhythmGame()
    {
        Camera.main.DOFieldOfView(60, 2);
        rhythmCircle1.SetActive(true);
        rhythmCircle2.SetActive(true);
        originalSize = rhythmCircle1.transform.localScale;
        rhythmCircle2.transform.localScale = originalSize * 5; // ������ �� rhythmCircle2�� ũ�⸦ rhythmCircle1�� 5��� ����ϴ�.
        rhythmCircle2.transform.DOScale(originalSize, 1).SetEase(Ease.InOutSine).OnUpdate(CheckClick);// 2�� ���� rhythmCircle2�� ũ�⸦ rhythmCircle1�� ũ��� ����ϴ�.
    }

    void CreatePlayerNBoss()
    {
        for (int i = 0; i < curPlayerCount; i++)
        {
            Vector3 newPos = new Vector3(playerLocation.position.x - i * 2, playerLocation.position.y, playerLocation.position.z);
            playerAll[i] = Instantiate(player, newPos, playerLocation.rotation);
        }

        for (int i = 0; i < curBossCount; i++)
        {
            Vector3 newPos = new Vector3(bossLocation.position.x + i * 2, bossLocation.position.y, bossLocation.position.z);
            bossAll[i] = Instantiate(boss, newPos, bossLocation.rotation);
        }

        currentPlayer = playerAll[0];
        currentBoss = bossAll[0];
    }

    void StartBattle()
    {
        StartCoroutine(MoveTowardsEachOther());
    }

    IEnumerator MoveTowardsEachOther()
    {
        currentPlayer.transform.position = new Vector2(playerLocation.position.x, playerLocation.position.y);
        currentBoss.transform.position = new Vector2(bossLocation.position.x, playerLocation.position.y);
        shouldCameraFollow = true;
        yield return new WaitForSeconds(1); //1�� �Ŀ� �̵� ����
        cam2UI.transform.DOMove(cam2Open.position, 1);
        currentPlayer.transform.DOMove(curPlayerLocation.position, 0.5f);
        currentBoss.transform.DOMove(curBossLocation.position, 0.5f);
        currentPlayer.transform.DOMove(bossLocation.transform.position, 7); //5�� ���� ������ ���� �̵�
        currentBoss.transform.DOMove(playerLocation.transform.position, 7); //5�� ���� �÷��̾ ���� �̵�
        yield return new WaitForSeconds(1.5f);
        cam2UI.transform.DOMove(cam2Close.position, 1);
        yield return new WaitForSeconds(0.5f);
        StartRhythmGame();
    }

    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            float distance = (rhythmCircle2.transform.localScale - originalSize).magnitude;
            if (distance < 0.2f)
            {
                Debug.Log("Fever!" + distance);
                state = State.Fever;
                int randomRange = Random.Range(2, 4);
                currentPlayer.transform.DOKill(); // �� �κ��� �߰�
                Vector3 destination = bossLocation.transform.position; //���� ��ġ
                //bossDummyLocation = new Vector2(currentBoss.transform.position.x, currentBoss.transform.position.y);
                destination.x += 10; //�ڷ� ���ư��� ��ġ. ���� ���� ����
                currentBoss.transform.DOKill(); // �� �κ��� �߰�
                currentBoss.transform.DOJump(destination, 10f, 1, 3f);
                StartCoroutine(FeverTime(randomRange));  // 2�� ���� �ǹ�Ÿ�� ����
            }
            else if (distance < 0.5f)
            {
                Debug.Log("Good!" + distance);
                state = State.Good;
                curBossCount--;
                Vector3 destination = bossLocation.transform.position; //���� ��ġ
                destination.x += 10; //�ڷ� ���ư��� ��ġ. ���� ���� ����
                currentBoss.transform.DOKill(); // �� �κ��� �߰�
                currentBoss.transform.DOJump(destination, 10f, 1, 3f); //���������� ���ư���. ù ��° �Ķ���ʹ� ������, �� ��°�� ���� ����, �� ��°�� ���� Ƚ��, �� ��°�� ��ü ���� �ð�
                currentPlayer.transform.DOKill(); // �� �κ��� �߰�
                currentPlayer.transform.DOMove(bossLocation.transform.position, 4); //5�� ���� ������ ���� �̵�
                StartCoroutine(DeactivateBoss());


                if (curBossCount > 0)
                {
                    currentBossIndex++;
                    currentBoss = bossAll[currentBossIndex];
                }
            }
            else
            {
                Debug.Log("Bad!" + distance);
                state = State.Bad;
                curPlayerCount--;
                Vector3 destination = playerLocation.transform.position; //���� ��ġ
                destination.x += -10; //�ݴ������� ���ư��� ��ġ. ���� ���� ����
                currentPlayer.transform.DOKill(); // �� �κ��� �߰�
                currentPlayer.transform.DOJump(destination, 10f, 1, 3f); //���������� ���ư���. ù ��° �Ķ���ʹ� ������, �� ��°�� ���� ����, �� ��°�� ���� Ƚ��, �� ��°�� ��ü ���� �ð�
                currentBoss.transform.DOKill(); // �� �κ��� �߰�
                currentBoss.transform.DOMove(playerLocation.transform.position, 4); //5�� ���� �÷��̾ ���� �̵�
                StartCoroutine(DeactivatePlayer());

                if (curPlayerCount > 0)
                {
                    currentPlayerIndex++;
                    currentPlayer = playerAll[currentPlayerIndex];
                }

                rhythmCircle2.transform.DOKill(); // ���� ũ�� ��ȭ Ʈ���� �����մϴ�.
            }
            rhythmCircle1.SetActive(false);
            rhythmCircle2.SetActive(false);

            Invoke("NextFight", 2f);
        }
    }

    IEnumerator DeactivateBoss()
    {
        GameObject a = currentBoss.gameObject;
        yield return new WaitForSeconds(1);
        a.SetActive(false);
        if(state == State.Good)
        {
            currentPlayer.transform.DOKill();
            currentPlayer.transform.DOMove(bossLocation.position, 0.01f);
        }
    }

    IEnumerator DeactivatePlayer()
    {
        GameObject a = currentPlayer.gameObject;
        yield return new WaitForSeconds(1);
        a.SetActive(false);
        if(state == State.Bad)
        {
            currentBoss.transform.DOKill();
            currentBoss.transform.DOMove(bossLocation.position, 0.01f);
        }
    }

    IEnumerator FeverTime(float duration)
    {
        isFeverTime = true;
        feverUI.SetActive(true);
        Debug.Log("�ǹ�Ÿ�� ����! " + duration);
        yield return new WaitForSeconds(duration);
        Debug.Log("�ǹ�Ÿ�� �� " + duration);
        isFeverTime = false;
        feverUI.SetActive(false);
    }

    void NextFight()
    {
        // �÷��̾ ���� Ŭ���ߴ��� üũ
        // ���� �ο� �Ǵ� �г�Ƽ �ο�
        if (curPlayerCount > 0 && curBossCount > 0)
        {

            isBattleongoing = true;
        }
        else
        {
            Debug.Log("GAMEOVER!");
            // game over
        }
    }
}
