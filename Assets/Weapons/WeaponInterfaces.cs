interface IRanged
{
    public void DisplayRange();
    public void HideRange();
}

interface ITargetsShip
{
    public void ShootShip (Ship targetShip);
    public int CalculateDamage(Ship targetShip);

}