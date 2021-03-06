﻿namespace LiveHAPI.Shared
{
    public class SqlAction
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Rank { get; set; }
        public string Action { get; set; }


        public SqlAction(decimal rank, string action)
        {
            Rank = rank;
            Action = action;
        }

        public SqlAction(decimal rank, string action, string name):this(rank,action)
        {
            Name = name;
        }

        public SqlAction(decimal rank, string action, string name, string description): this(rank, action,name)
        {
            Description = description;
        }
    }
}
