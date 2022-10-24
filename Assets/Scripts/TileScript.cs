using UnityEngine;

public enum TileState { Black, White, Empty };

public class TileScript : MonoBehaviour
{
    public Material highlightMaterial;
    [SerializeField] private Material groundMaterial;
    [SerializeField] private TileState tileState;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform pawn;
    [SerializeField] private Renderer gRenderer;
    private Index _index;

    private void Awake()
    {
        gRenderer = gameObject.GetComponentInChildren<Transform>().Find("Ground").GetComponent<Renderer>();
        groundMaterial = gRenderer.material;
        animator = gameObject.GetComponentInChildren<Transform>().Find("Pawn").GetComponent<Animator>();
        pawn = gameObject.GetComponentInChildren<Transform>().Find("Pawn").transform;
        
        PlaceTile(TileState.Empty, false);
    }

    private void OnMouseOver() => gRenderer.material = tileState == TileState.Empty ? highlightMaterial : groundMaterial;

    private void OnMouseExit() => gRenderer.material = groundMaterial;
    
    public void PlaceTile(TileState stateToTurn, bool sound)
    {
        pawn.gameObject.SetActive(true);
        switch (stateToTurn)
        {
            case TileState.Black:
                animator.Play("PlaceBlack");
                break;
            case TileState.White:
                animator.Play("PlaceWhite");
                break;
            case TileState.Empty:
                pawn.gameObject.SetActive(false);
                break;
        }
        tileState = stateToTurn;
    }
    public void TurnTile(TileState stateToTurn)
    {

        pawn.gameObject.SetActive(true);
        switch (stateToTurn)
        {
            case TileState.Black:
                animator.Play("FlipToBlack");
                break;
            case TileState.White:
                animator.Play("FlipToWhite");
                break;
            case TileState.Empty:
                pawn.gameObject.SetActive(false);
                break;
        }
        tileState = stateToTurn;
    }
    public TileState GetTileState()
    {
        return tileState;
    }
    public Index GetIndex()
    {
        return _index;
    }
    public void SetIndex(int z, int x)
    {
        _index = new Index(z, x);
    }
}
