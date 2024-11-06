
using UdonSharp;

public class AlchemistManager : UdonSharpBehaviour
{
    public AlchemistObjectManager alchemistObjectManager;

    private void OnEnable()
    {
            alchemistObjectManager.SetAsAlchemist();
    }

    private void OnDisable()
    {
            alchemistObjectManager.SetAsNotAlchemist();
    }
}