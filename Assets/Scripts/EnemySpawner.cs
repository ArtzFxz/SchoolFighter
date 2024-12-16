using Assets.Scripts;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject[] enemyArray;

    public int numberOfEnemies;
    private int currentEnemies;

    public float spawnTime;

    public string NextSection;


    void Update()
    {
        // Caso atinja o numero maximo de inimigos spawnados
        if (currentEnemies >= numberOfEnemies)
        {
            // Contar a quantidade de inimigos ativos na cena
            int enemies = FindObjectsByType<EnemyMeleeController>(FindObjectsSortMode.None).Length;

            if (enemies <= 0)
            {
                // Avança de seçao
                LevelManager.ChangeSection(NextSection);

                // Desabilita o spawner assim que os inimigos morrerem
                this.gameObject.SetActive(false);
            }
        }
    }

    void SpawnEnemy()
    {
        // Posicao de Spawn do inimigo
        Vector2 spawnPosition;

        // Limite Y
        // -0.36
        // -0.95
        spawnPosition.y = Random.Range(-0.95f, -0.36f);

        //Posicao X máximo (direita) do confiner da camera + 1 de distancia.
        //Pegar RightBound (limite direito) da Section (Confiner) como base.
        float rightSectionBound = LevelManager.currentConfiner.BoundingShape2D.bounds.max.x;

        //Define o x do spawnPosition, igual ao ponto da DIREITA do confiner.
        spawnPosition.x = rightSectionBound;

        // Instancia (SPAWNA) os inimigos
        // Pega um inimigo aleatorio da lista de inimigos
        // Spawna na posicao spawnPosition
        // Quaternion é uma classe utilizada para trabalhar com rotaçoes
        Instantiate(enemyArray[Random.Range(0, enemyArray.Length)], spawnPosition, Quaternion.identity).SetActive(true);

        // Incrementa o contador de inimigos do spawner.
        currentEnemies++;

        // Controlar o número de inimigos, se o numero de inimigos na cena for menor que o max dos inimigos,
        // Invoca novamente a funçao de spawn.
        if (currentEnemies < numberOfEnemies)
        {
            Invoke("SpawnEnemy", spawnTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.GetComponent<PlayerController>();

        if (player)
        {
            // Desativa o colisor para iniciar o spawning apenas uma vez
            // ATENÇAO: Desabilita o collider, mas o objeto spawner continua ativo
            this.GetComponent<BoxCollider2D>().enabled = false;

            // Invoca pela primeira vez a funçao SpawnEnemy
            SpawnEnemy();
        }
    }
}
