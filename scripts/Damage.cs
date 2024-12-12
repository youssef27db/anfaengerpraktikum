using Godot;

/** 
 * @brief Repräsentiert den Schaden, der von Charakteren oder Gegnern verursacht wird.
 * Beinhaltet physischen Schaden, wahren Schaden und den Rückstoßeffekt.
 */
public class Damage{

    private float PhysicalDMG;
    private float TrueDMG;
    private Vector2 PushAmount;
    private Node2D Source;

    /** 
     * @brief Konstruktor für die Damage-Klasse.
     * @param PhysicalDMG Der physische Schaden.
     * @param TrueDMG Der wahre Schaden.
     * @param PushAmount Der Rückstoßvektor.
     */
    public Damage(float PhysicalDMG, float TrueDMG, Vector2 PushAmount, Node2D Source){
        this.PhysicalDMG = PhysicalDMG;
        this.TrueDMG = TrueDMG;
        this.PushAmount = PushAmount;
        this.Source = Source;
    }

    /** 
     * @brief Gibt den physischen Schaden zurück.
     * @return Der physische Schaden.
     */
    public float GetPhysicalDMG(){
        return PhysicalDMG;
    }

    /** 
     * @brief Gibt den wahren Schaden zurück.
     * @return Der wahre Schaden.
     */
    public float GetTrueDMG(){
        return TrueDMG;
    }

    /** 
     * @brief Gibt den Rückstoßvektor zurück.
     * @return Der Rückstoßvektor.
     */
    public Vector2 GetPushAmount(){
        return PushAmount;
    }

    /**
    * @brief Gibt die Ursache zurück.
    * @return Die Ursache.
    */
    public Node2D GetSource(){
        return Source;
    }
}