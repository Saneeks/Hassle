using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hassle : MonoBehaviour
{
    public GameObject hassleBarLink;
    public GameObject camera;

    private Camera cameraSize;
    private float cameraOriginalSize;
    private GameObject player;
    private Rigidbody2D playerRB;
    private Collider2D playerCollider;
    private Vector2 playerPosition;
    private GameObject enemy;
    private Rigidbody2D enemyRB;
    private Collider2D enemyCollider;
    private Vector2 enemyPosition;
    private Vector2 centerOfHassle;
    private GameObject hassleBar;
    private HassleStatus hassleStatus;

    private int impulseForce = 4;
    private bool hassleIsBlocked = false;
    private bool inHassle = false;
    private bool balanceIsChanged = false;
    private int playerForce = 10, enemyForce = 10;
    private int time = 4;

    private void PlayerHit() // Толчок игрока
    {
        playerForce++;
        enemyForce--;
        balanceIsChanged = true;
    }
    private void EnemyHit() // Толчок врага (авто)
    {
        playerForce--;
        enemyForce++;
        balanceIsChanged = true;
    }
    private void Timer() // Просто таймер (для отсчёта длины стычки)
    {
        time--;
    }
    private void UnblockHassle() // Задержка разблокировки для следующей тычки
    {
        hassleIsBlocked = false;
    }
    void Start() // Инициализация
    {
        player = gameObject;
        playerRB = player.GetComponent<Rigidbody2D>();
        playerCollider = player.GetComponent<Collider2D>();
        cameraSize = camera.GetComponent<Camera>();
    }

    void Update()
    {
        if (inHassle)
        {
            if (Input.GetMouseButtonDown(0)) // Клик игрока
            {
                PlayerHit();
            }
            if (balanceIsChanged) // Отрисовка полосы
            {
                hassleStatus.SetBalance(playerForce, enemyForce);
                balanceIsChanged = false;
            }
            if (playerForce == 0 || enemyForce == 0 || time == 0) // Конец стычки
            {
                // Востановление параметров
                CancelInvoke("Timer");
                CancelInvoke("EnemyHit");
                playerRB.constraints = RigidbodyConstraints2D.None;
                playerCollider.enabled = true;
                enemyRB.constraints = RigidbodyConstraints2D.None;
                enemyCollider.enabled = true;

                if (playerForce == 0) // Победа врага
                {
                    // ЗАМЕНИТЬ НА ПОБЕДУ ВРАГА
                    if (playerPosition.x > enemyPosition.x)
                    {
                        playerRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        playerRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                    }
                    Debug.Log("Red win");
                }
                if (enemyForce == 0) // Победа игрока
                {
                    // ЗАМЕНИТЬ НА ПОБЕДУ ИГРОКА
                    if (playerPosition.x > enemyPosition.x)
                    {
                        playerRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        playerRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                    }
                    Debug.Log("Blue win");
                }
                if (time == 0) // Ничья
                {
                    // ЗАМЕНИТЬ НА УРОН ОБЕИМ СТОРОНАМ
                    if (playerPosition.x > enemyPosition.x) // Расставляем объекты для стычки
                    {
                        playerRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                    }
                    else
                    {
                        playerRB.AddForce(transform.right * -impulseForce, ForceMode2D.Impulse);
                        enemyRB.AddForce(transform.right * impulseForce, ForceMode2D.Impulse);
                    }
                    Debug.Log("Draft");
                }

                // Востановление параметров
                Destroy(hassleBar);
                cameraSize.orthographicSize = cameraOriginalSize;
                balanceIsChanged = false;
                inHassle = false;
                time = 3;
                Invoke("UnblockHassle", 3);
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && !hassleIsBlocked)
        {
            hassleIsBlocked = true;
            inHassle = true; // Запрет на стычки

            enemy = collision.gameObject;                           // Получаем оппонента
            enemyRB = enemy.GetComponent<Rigidbody2D>();            // Получаем RigidBody
            enemyRB.constraints = RigidbodyConstraints2D.FreezeAll; // Замораживаем врага
            enemyCollider = enemy.GetComponent<Collider2D>();       // Получаем Collider
            enemyCollider.enabled = false;                          // Отключаем Collider

            playerRB.constraints = RigidbodyConstraints2D.FreezeAll;// Замораживаем игрока
            playerCollider.enabled = false;                         // Отключаем Collider

            playerPosition = player.transform.position;             // Записываем координаты игрока
            enemyPosition = enemy.transform.position;               // Записываем координаты врага
            centerOfHassle = enemyPosition / 2 + playerPosition / 2;// Находим центр стычки


            if (playerPosition.x > enemyPosition.x) // Расставляем объекты для стычки
            {
                player.transform.position = new Vector2(centerOfHassle.x + 0.5f, centerOfHassle.y);
                enemy.transform.position = new Vector2(centerOfHassle.x - 0.5f, centerOfHassle.y);
            }
            else
            {
                player.transform.position = new Vector2(centerOfHassle.x - 0.5f, centerOfHassle.y);
                enemy.transform.position = new Vector2(centerOfHassle.x + 0.5f, centerOfHassle.y);
            }

            hassleBar = Instantiate(hassleBarLink, new Vector2(centerOfHassle.x, centerOfHassle.y + 1), Quaternion.identity); // Создание полосы стычки
            hassleStatus = hassleBar.GetComponent<HassleStatus>(); // Получение скрипта полосы

            playerForce = 10;                                   // Задаются базовые парамметры для стычки
            enemyForce = 10;
            hassleStatus.SetBalance(playerForce, enemyForce);   // Отрисовывается ползунок

            camera.transform.position = new Vector3(centerOfHassle.x, centerOfHassle.y, camera.transform.position.z);   // Камера занимает позицию центра стычки
            cameraOriginalSize = cameraSize.orthographicSize;   // Запись предыдущего положения камеры
            cameraSize.orthographicSize = 3;    // Приближает изображение

            InvokeRepeating("EnemyHit", 1, 0.2f);   // Включается автоклик у врага
            InvokeRepeating("Timer", 1, 1f);      // Включается таймер на стычку
        }
    }
}
