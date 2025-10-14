using UnityEngine;

public class EnemyComponentControls : MonoBehaviour
{
    protected EnemyConfiguration config;
    protected Enemy enemy;
    public virtual void Setup(EnemyConfiguration config, Enemy enemy)
    {
        this.config = config;
        this.enemy = enemy;
    }

    public virtual void UpdateLookInput(Vector2 input) { }
    public virtual void UpdateMoveInput(Vector2 moveInput, bool jumpInput, bool sprintInput) { }
    public virtual void UpdateFireInput(bool leftInput, bool rightInput, bool reloadButton) { }
    public virtual void UpdateFixedPhysics() { }
    public virtual void UpdatePrevNextInput(bool prev, bool next) { }

    protected Vector3 EnemyUp
    {
        get
        {
            return enemy.EnemyUp;
        }
    }
    protected float EnemyDT => enemy.DeltaTime;
    protected float EnemyFixedDT => enemy.FixedDeltaTime;
}
