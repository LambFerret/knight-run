using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    public Transform mainCam; //메인캠
    public Transform cam2; //캠
    public GameObject cam2UI;

    private bool shouldCameraFollow = true; // 카메라가 플레이어를 따라갈지 여부

    public int curPlayerCount; //현재 플레이어 카운트
    public int curBossCount; //현재 보스 카운트

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

    public bool isBattleongoing; //배틀이 시작되었는가?
    public bool isstop; //배틀이 시작되었는가?
    bool isFeverTime = false;

    private Vector3 originalSize; // rhythmCircle1의 원래 크기를 저장

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
        
        if (isFeverTime && Input.GetMouseButtonDown(0) && curTouchTime > 0.1f) //피버타임
        {
            
            curBossCount--;
            Debug.Log("bossDummyLocation: " + bossDummyLocation);
            Vector3 destination = bossLocation.transform.position; //원래 위치
            int b = Random.Range(7, 20);
            bossDummyLocation = new Vector2(currentPlayer.transform.position.x + 2, currentPlayer.transform.position.y);
            destination.x += 10; //뒤로 날아가는 위치. 값은 조정 가능
            currentBoss.transform.DOJump(destination, b, 1, 3f); //포물선으로 날아가기. 첫 번째 파라미터는 목적지, 두 번째는 점프 높이, 세 번째는 점프 횟수, 네 번째는 전체 동작 시간
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
        rhythmCircle2.transform.localScale = originalSize * 5; // 시작할 때 rhythmCircle2의 크기를 rhythmCircle1의 5배로 만듭니다.
        rhythmCircle2.transform.DOScale(originalSize, 1).SetEase(Ease.InOutSine).OnUpdate(CheckClick);// 2초 동안 rhythmCircle2의 크기를 rhythmCircle1의 크기로 만듭니다.
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
        yield return new WaitForSeconds(1); //1초 후에 이동 시작
        cam2UI.transform.DOMove(cam2Open.position, 1);
        currentPlayer.transform.DOMove(curPlayerLocation.position, 0.5f);
        currentBoss.transform.DOMove(curBossLocation.position, 0.5f);
        currentPlayer.transform.DOMove(bossLocation.transform.position, 7); //5초 동안 보스를 향해 이동
        currentBoss.transform.DOMove(playerLocation.transform.position, 7); //5초 동안 플레이어를 향해 이동
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
                currentPlayer.transform.DOKill(); // 이 부분을 추가
                Vector3 destination = bossLocation.transform.position; //원래 위치
                //bossDummyLocation = new Vector2(currentBoss.transform.position.x, currentBoss.transform.position.y);
                destination.x += 10; //뒤로 날아가는 위치. 값은 조정 가능
                currentBoss.transform.DOKill(); // 이 부분을 추가
                currentBoss.transform.DOJump(destination, 10f, 1, 3f);
                StartCoroutine(FeverTime(randomRange));  // 2초 동안 피버타임 시작
            }
            else if (distance < 0.5f)
            {
                Debug.Log("Good!" + distance);
                state = State.Good;
                curBossCount--;
                Vector3 destination = bossLocation.transform.position; //원래 위치
                destination.x += 10; //뒤로 날아가는 위치. 값은 조정 가능
                currentBoss.transform.DOKill(); // 이 부분을 추가
                currentBoss.transform.DOJump(destination, 10f, 1, 3f); //포물선으로 날아가기. 첫 번째 파라미터는 목적지, 두 번째는 점프 높이, 세 번째는 점프 횟수, 네 번째는 전체 동작 시간
                currentPlayer.transform.DOKill(); // 이 부분을 추가
                currentPlayer.transform.DOMove(bossLocation.transform.position, 4); //5초 동안 보스를 향해 이동
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
                Vector3 destination = playerLocation.transform.position; //원래 위치
                destination.x += -10; //반대편으로 날아가는 위치. 값은 조정 가능
                currentPlayer.transform.DOKill(); // 이 부분을 추가
                currentPlayer.transform.DOJump(destination, 10f, 1, 3f); //포물선으로 날아가기. 첫 번째 파라미터는 목적지, 두 번째는 점프 높이, 세 번째는 점프 횟수, 네 번째는 전체 동작 시간
                currentBoss.transform.DOKill(); // 이 부분을 추가
                currentBoss.transform.DOMove(playerLocation.transform.position, 4); //5초 동안 플레이어를 향해 이동
                StartCoroutine(DeactivatePlayer());

                if (curPlayerCount > 0)
                {
                    currentPlayerIndex++;
                    currentPlayer = playerAll[currentPlayerIndex];
                }

                rhythmCircle2.transform.DOKill(); // 원의 크기 변화 트윈을 중지합니다.
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
        Debug.Log("피버타임 시작! " + duration);
        yield return new WaitForSeconds(duration);
        Debug.Log("피버타임 끝 " + duration);
        isFeverTime = false;
        feverUI.SetActive(false);
    }

    void NextFight()
    {
        // 플레이어가 원을 클릭했는지 체크
        // 점수 부여 또는 패널티 부여
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
