namespace LetsSport.Common
{
    using System.Collections.Concurrent;

    public class SportImageUrl
    {
        private ConcurrentDictionary<string, string> sportUrls;

        public SportImageUrl()
        {
            this.sportUrls = new ConcurrentDictionary<string, string>
            {
                ["Basketball"] = "https://images.unsplash.com/photo-1516802273409-68526ee1bdd6?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Football"] = "https://images.unsplash.com/photo-1486286701208-1d58e9338013?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Volleyball"] = "https://images.unsplash.com/photo-1553005746-9245ba190489?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=1050&q=80",
                ["Aquatics"] = "https://images.unsplash.com/photo-1519209233471-a93512eebb72?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Archery"] = "https://images.unsplash.com/photo-1538432091670-e6b79bd9bffa?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Athletics"] = "https://images.unsplash.com/photo-1526676037777-05a232554f77?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Badminton"] = "https://images.unsplash.com/photo-1564226803380-91139fdcb4d0?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Baseball"] = "https://images.unsplash.com/photo-1525571296628-8c2ee4e47321?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Boxing"] = "https://images.unsplash.com/photo-1495555687398-3f50d6e79e1e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["CanoeKayak"] = "https://images.unsplash.com/photo-1544407558-71e53c6a1136?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Cycling"] = "https://images.unsplash.com/photo-1541625602330-2277a4c46182?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Equestrian"] = "https://images.unsplash.com/photo-1526038039141-92d734991065?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Fencing"] = "https://images.unsplash.com/photo-1505619656705-59ebc350b547?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["FieldHockey"] = "https://images.unsplash.com/photo-1537753068441-ae5962264fe7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Golf"] = "https://images.unsplash.com/photo-1535132011086-b8818f016104?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Gymnastics"] = "https://images.unsplash.com/photo-1516208813382-cbaf53501037?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Handball"] = "https://images.unsplash.com/photo-1553627220-92f0446b6a5f?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["IceHockey"] = "https://images.unsplash.com/photo-1581275701366-d9bfe401faa4?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Judo"] = "https://images.unsplash.com/photo-1542937307-6eeb0267cbab?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Karate"] = "https://images.unsplash.com/photo-1529630218527-7df22fc2d4ee?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Rowing"] = "https://images.unsplash.com/photo-1558253917-0edb67da60e5?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Rugby"] = "https://images.unsplash.com/photo-1480099225005-2513c8947aec?ixlib=rb-1.2.1&auto=format&fit=crop&w=320&q=80",
                ["Sailing"] = "https://images.unsplash.com/photo-1506527240747-720a3e17b910?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Shooting"] = "https://images.unsplash.com/photo-1562461094-e060ef34728e?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Softball"] = "https://images.unsplash.com/photo-1549840962-3f0db68248e2?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Squash"] = "https://images.unsplash.com/photo-1554290813-ec6a2a72e5c7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["TableTennis"] = "https://images.unsplash.com/photo-1461748659110-16121c049d52?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Taekwondo"] = "https://images.unsplash.com/photo-1514050566906-8d077bae7046?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Tennis"] = "https://images.unsplash.com/photo-1554068865-24cecd4e34b8?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Triathlon"] = "https://images.unsplash.com/photo-1533547477463-bcffb9ef386d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Weightlifting"] = "https://images.unsplash.com/photo-1521804906057-1df8fdb718b7?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
                ["Wrestling"] = "https://images.unsplash.com/photo-1541337082051-5959dbb57d5d?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=320&q=80",
            };
        }

        public string GetSportPath(string sportName) => this.sportUrls[sportName];
    }
}
