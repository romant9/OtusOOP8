namespace Collections
{
    public class CustomLinkedList
    {
        private Link _currentLink;

        public void Add(string value)
        {
            var link = new Link();
            link.Value = value;
            link.PreviousLink = _currentLink;
            
            _currentLink = link;
        }

        public string GetValueAt(int i)
        {
            var index = i;
            var tempLink = _currentLink;
            while (true)
            {
                if (tempLink == null)
                    return null;

                if (index == 0)
                    return tempLink.Value;

                index -= 1;
                tempLink = _currentLink.PreviousLink;
            }
        }
    }

    public class Link
    {
        public string Value;
        public Link PreviousLink;
    }
}