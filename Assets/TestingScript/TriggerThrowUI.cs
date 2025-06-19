using UnityEngine;
using UnityEngine.EventSystems;

public class TriggerThrowUI : MonoBehaviour, IPointerEnterHandler
{
    
    private TestAddForceToBlock _testAddForceToBlock;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _testAddForceToBlock = GameObject.FindWithTag("Player").GetComponent<TestAddForceToBlock>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (CompareTag("LeftTriggerUI") == true)
        {
            if (_testAddForceToBlock.blockX > _testAddForceToBlock.transform.position.x || _testAddForceToBlock.blockIsGround == false) 
            {
                _testAddForceToBlock.Throw();
            }
           
        }

        if (CompareTag("RightTriggerUI") == true)
        {
            if (_testAddForceToBlock.blockX < _testAddForceToBlock.transform.position.x || _testAddForceToBlock.blockIsGround == false) 
            {
                _testAddForceToBlock.Throw();
            }
            
        }

        //if (CompareTag("LeftTriggerUI") == true)
        //{
        //    _testAddForceToBlock.Throw();
        //}

        //if (CompareTag("RightTriggerUI") == true)
        //{
        //    _testAddForceToBlock.Throw();
        //}


    }


}
