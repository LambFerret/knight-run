using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossManager : MonoBehaviour
{
    public Image blackSquare;

    public TMP_Text curAllyCountUI;
    public TMP_Text curBossCountUI;

    public Transform mainCam; //메인캠
    public Transform cam2; //캠
    public GameObject cam2UI;

    public GameObject paching;
    public GameObject crown;

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

    public bool isBattleongoing = true; //배틀이 시작되었는가?
    public bool isstop; //배틀이 시작되었는가?
    public bool isstopCheck; //배틀이 시작되었는가?
    public bool isDual; //리듬듀얼중인가?
    public bool isClicked; //클릭했는가?
    bool isFeverTime = false;

    private Vector3 originalSize; // rhythmCircle1의 원래 크기를 저장

    public int currentPlayerIndex = 0;
    public int currentBossIndex = 0;

    float curTouchTime;

    public bool isWin;
    public bool isWinCheck;

    public enum State
    {
        None, Fever, Bad, Good
    }

    bool turnDone;

    public State state;

    public Vector2 bossDummyLocation;

    //UI
    public TMP_Text feverUI;

    void Start()
    {
        StartCoroutine(FadeOutAndDisable());
        feverUI.gameObject.SetActive(false);
        curPlayerCount = 10;
        curBossCount = 30;
        playerAll = new GameObject[curPlayerCount];
        bossAll = new GameObject[curBossCount];
        CreatePlayerNBoss();
        isBattleongoing = false;
    }

    IEnumerator FadeOutAndDisable()
    {
        // Fade out over 1.5 seconds
        blackSquare.DOFade(0f, 1.5f);
        yield return new WaitForSeconds(1.5f);

        // After 1.5 seconds, disable the square
        blackSquare.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBattleongoing && !isWin)
        {
            StartBattle();
            isBattleongoing = true;
            isstop = false;
            isstopCheck = false;
        }
        CameraFollow();
        if(isDual) CheckClick();
        
        if (isFeverTime && Input.GetMouseButtonDown(0) && curTouchTime > 0.1f && curBossCount > 0) //피버타임
        {
            Camera.main.transform.DOShakePosition(0.4f, 0.4f, 2, 180);
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

        if(!isFeverTime && curBossCount <= 0 && !isWinCheck)
        {
            Debug.Log("Winner!");
            isWin = true;
            isWinCheck = true;
            Vector2 crownFirstPosition = new Vector2(currentPlayer.transform.position.x, currentPlayer.transform.position.y + 20);
            crown.transform.position = crownFirstPosition;
            crown.transform.DOMove(currentPlayer.transform.position, 7);
        }

    }

    private void LateUpdate()
    {
        curAllyCountUI.text = "X" + curPlayerCount;
        curBossCountUI.text = "X" + curBossCount;
    }

    void CameraFollow()
    {
        // Check if currentBoss is close to currentPlayer
        float distance = (currentPlayer.transform.position - currentBoss.transform.position).magnitude;

        if (distance < 10.0f)  // you can adjust this value
        {
            // If the currentBoss is close, stop the camera
            shouldCameraFollow = false;
            if (!isstop && !isstopCheck)
            {
                isstop = true;
                isstopCheck = true;
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
        Camera.main.DOFieldOfView(40, 1);
        rhythmCircle1.SetActive(true);
        rhythmCircle2.SetActive(true);
        Vector3 pachingPosition = currentPlayer.transform.position + new Vector3(3, 0, 0);
        Instantiate(paching, pachingPosition, Quaternion.identity);

        originalSize = rhythmCircle1.transform.localScale;
        rhythmCircle2.transform.localScale = originalSize * 5; // 시작할 때 rhythmCircle2의 크기를 rhythmCircle1의 5배로 만듭니다.
        rhythmCircle2.transform.DOScale(originalSize, 1).SetEase(Ease.InOutSine);// 2초 동안 rhythmCircle2의 크기를 rhythmCircle1의 크기로 만듭니다.
        isDual = true;
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
        //Camera.main.transform.DOKill();
        //Camera.main.DOFieldOfView(60, 0.01f);
        StartCoroutine(MoveTowardsEachOther());
    }

    IEnumerator MoveTowardsEachOther()
    {
        currentPlayer.transform.position = new Vector2(playerLocation.position.x, playerLocation.position.y);
        currentBoss.transform.position = new Vector2(bossLocation.position.x, bossLocation.position.y);
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
        float distance = (rhythmCircle2.transform.localScale - originalSize).magnitude;
        if (Input.GetMouseButtonDown(0))
        {
            isClicked = true;
            isstop = false;
            isDual = false;
            if (distance < 0.3f && distance > 0)
            {
                Debug.Log("Fever!" + distance);
                state = State.Fever;
                //int randomRange = Random.Range(2, 4);
                currentPlayer.transform.DOKill(); // 이 부분을 추가
                Vector3 destination = bossLocation.transform.position; //원래 위치
                //bossDummyLocation = new Vector2(currentBoss.transform.position.x, currentBoss.transform.position.y);
                destination.x += 10; //뒤로 날아가는 위치. 값은 조정 가능
                currentBoss.transform.DOKill(); // 이 부분을 추가
                currentBoss.transform.DOJump(destination, 10f, 1, 3f);
                StartCoroutine(FeverTime(2));  // 2초 동안 피버타임 시작
                Camera.main.DOKill();
                Camera.main.DOFieldOfView(60, 0.5f);
                Camera.main.transform.DOShakePosition(1, 1, 5, 180);
            }
            else if (distance < 1.3f && distance >= 0.3f)
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
                Camera.main.DOKill();
                Camera.main.DOFieldOfView(60, 0.5f);
                Camera.main.transform.DOShakePosition(1, 1, 5, 180);

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
                Camera.main.DOKill();
                Camera.main.DOFieldOfView(60, 0.5f);
                Camera.main.transform.DOShakePosition(1, 1, 5, 180);

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
        if (distance <= 0)
        {
            if (!isClicked)
            {
                isClicked = false;
                isstop = false;
                isDual = false;
                rhythmCircle1.SetActive(false);
                rhythmCircle2.SetActive(false);
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
                Camera.main.DOKill();
                Camera.main.DOFieldOfView(60, 0.5f);
                Camera.main.transform.DOShakePosition(1, 1, 5, 180);

                if (curPlayerCount > 0)
                {
                    currentPlayerIndex++;
                    currentPlayer = playerAll[currentPlayerIndex];
                }

                rhythmCircle2.transform.DOKill(); // 원의 크기 변화 트윈을 중지합니다.

                Invoke("NextFight", 2f);
            }
            else
            {
                isDual = false;
                isClicked = false;
                isstop = false;
            }

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
        feverUI.gameObject.SetActive(true);
        int a = (int)duration * 5;
        StartCoroutine(FeverUIShow());
        Debug.Log("피버타임 시작! " + duration);
        yield return new WaitForSeconds(duration);
        Debug.Log("피버타임 끝 " + duration);
        currentBoss.transform.DOKill();
        currentBoss.transform.DOMove(bossLocation.transform.position, 0.01f);
        isFeverTime = false;
        feverUI.gameObject.SetActive(false);
    }

    IEnumerator FeverUIShow()
    {
        // 네온 색상 정의
        Color neonBlue = new Color(77f / 255f, 77f / 255f, 255f / 255f);
        Color neonGreen = new Color(57f / 255f, 255f / 255f, 20f / 255f);

        while (isFeverTime)
        {
            feverUI.transform.DOScale(1.2f, 0.2f); // feverUI를 0.2초 동안 1.5배로 키우기
            feverUI.DOColor(neonBlue, 0.2f);
            yield return new WaitForSeconds(0.2f);
            feverUI.DOColor(neonGreen, 0.2f);
            feverUI.transform.DOScale(1f, 0.2f); // feverUI를 다시 0.2초 동안 원래 크기로 줄이기
            yield return new WaitForSeconds(0.2f);
        }
        feverUI.gameObject.SetActive(false);
    }

    void NextFight()
    {
        // 플레이어가 원을 클릭했는지 체크
        // 점수 부여 또는 패널티 부여
        if (curPlayerCount > 0 && curBossCount > 0)
        {
            Camera.main.DOKill();
            Camera.main.DOFieldOfView(60, 0.5f);
            isBattleongoing = false;
        }
        else
        {
            Debug.Log("GAMEOVER!");
            // game over
        }
    }
}
