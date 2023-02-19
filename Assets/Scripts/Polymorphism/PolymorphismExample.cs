using System.Collections.Generic;
using UnityEngine;

namespace Polymorphism
{
    public class PolymorphismExample : MonoBehaviour
    {
        #region Parametric polymorphism for data types and functions

        [ContextMenu("Parametric")]
        public void Parametric()
        {
            // jTODO take a look inside 'List' type
            List<int> listOfInts = new List<int>();
            List<MonoBehaviour> listOfIntMonoBehaviours = new List<MonoBehaviour>();

            var stringTypeName = GetTypeName("word");
            var floatTypeName = GetTypeName(1f);
            var intTypeName = GetTypeName(1);

            Debug.Log($"PolymorphismExample.Parametric: {stringTypeName}, {floatTypeName}, {intTypeName}");
        }

        private string GetTypeName<T>(T type)
        {
            return typeof(T).FullName;
        }

        #endregion


        #region Subtyping

        [ContextMenu("Subtyping")]
        public void Subtyping()
        {
            Character character = new Character();
            Character friendlyNPC = new FriendlyNPC();
            Character aggressiveNPC = new AggressiveNPC();

            Debug.Log($"PolymorphismExample.Subtyping: {character.SayHello()}, {friendlyNPC.SayHello()}, {aggressiveNPC.SayHello()}");
        }

        private class Character
        {
            public virtual string SayHello()
            {
                return "Hello!";
            }
        }

        private class FriendlyNPC : Character
        {
            public override string SayHello()
            {
                return "Howdy!";
            }
        }

        private class AggressiveNPC : Character
        {
            public override string SayHello()
            {
                return "Go away!";
            }
        }

        #endregion


        #region Ad hoc: overload

        [ContextMenu("Ad hoc: overload")]
        public void Overload()
        {
            var isPlayableGame = IsPlayable(new Game());
            var isPlayableGameByName = IsPlayable("Best Game Ever");
            var isPlayableAudioDevice = IsPlayable(new AudioDevice());

            Debug.Log($"PolymorphismExample.Overload: {isPlayableGame}, {isPlayableGameByName}, {isPlayableAudioDevice}");
        }

        private class Game
        {
        }

        private class AudioDevice
        {
        }

        private bool IsPlayable(Game game)
        {
            return true;
        }

        private bool IsPlayable(string gameName)
        {
            return true;
        }

        private bool IsPlayable(AudioDevice audioDevice)
        {
            return true;
        }

        #endregion

        #region Ad hoc: cast

        [ContextMenu("Ad hoc: cast")]
        public void Cast()
        {
            var sumFloatsWithFloats = SumFloats(1f, 2f);
            var sumFloatsWithInts = SumFloats(3, 4);

            // jIMPORTANT Need explicit cast
            // var sumIntsWithFloats = SumInts(5f, 6f);
            var sumIntsWithInts = SumInts(7, 8);

            Debug.Log($"PolymorphismExample.Cast: {sumFloatsWithFloats}, {sumFloatsWithInts}, {sumIntsWithInts}");
        }

        private float SumFloats(float a, float b)
        {
            return a + b;
        }

        private float SumInts(int a, int b)
        {
            return a + b;
        }

        #endregion
    }
}