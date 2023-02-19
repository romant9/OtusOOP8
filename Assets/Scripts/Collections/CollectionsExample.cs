using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Collections
{
    public class CollectionsExample : MonoBehaviour
    {
        [ContextMenu("UseCollections")]
        public void UseCollections()
        {
            List<int> listOfInts = new List<int>();
            List<MonoBehaviour> listOfIntMonoBehaviours = new List<MonoBehaviour>();

            var queue = new Queue();
            queue.Enqueue(1);
            queue.Enqueue(2);
            queue.Enqueue(3);
            queue.Dequeue();

            var stack = new Stack<string>(5);
            stack.Push("1");
            stack.Push("2");
            stack.Push("3");
            var stackPeek = stack.Peek();
            var stackPop = stack.Pop();

            Dictionary<MonoBehaviour, string> dictionary = new Dictionary<MonoBehaviour, string>();
            dictionary.Add(this, "This is a description for this component");

            var linkedList = new LinkedList<int>();
            var firstNode = linkedList.AddFirst(1);
            var lastNode = linkedList.AddLast(9);
            var preLastNode = linkedList.AddBefore(lastNode, 8);
            var secondNode = linkedList.AddAfter(firstNode, 2);

            var customLinkedList = new CustomLinkedList();
            customLinkedList.Add("Apple");
            customLinkedList.Add("Pear");
            customLinkedList.Add("Banana");
            customLinkedList.GetValueAt(2);
        }
    }
}