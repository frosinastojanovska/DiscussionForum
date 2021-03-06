﻿using System;

namespace DiscussionForum.Domain.DomainModel
{
    public class TopicFollower
    {
        public TopicFollower(int topicId, int followerId)
        {
            TopicID = topicId;
            FollowerID = followerId;
        }
        public int ID { get; private set; }
        public int TopicID { get; private set; }
        public int FollowerID { get; private set; }
    }
}