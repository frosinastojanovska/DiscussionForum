﻿using Dapper;
using DiscussionForum.Domain.DomainModel;
using DiscussionForum.Services.DTOs;
using DiscussionForum.Services.Interfaces;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DiscussionForum.Services
{
    public class TopicService : ITopicService
    {
        private IDbConnection _connection { get; set; }

        public TopicService(IDbConnection connection)
        {
            _connection = connection;
        }

        public void CreateTopic(Topic topic)
        {
            string query = "INSERT INTO Topics (CreatorID, CategoryID, Title, Description, Reported, Closed, DateCreated, LastActivity)" +
            "values(@CreatorID, @CategoryID, @Title, @Description, @Reported, @Closed, @DateCreated, @LastActivity)";
            _connection.Execute(query, new { topic.CreatorID, topic.CategoryID, topic.Title, topic.Description, topic.Reported, topic.Closed, topic.DateCreated, topic.LastActivity });
        }

        public TopicDTO GetTopicById(int topicID, int Id)
        {

            var sqlQueryTopic = $@"SELECT
                                   Topics.ID                AS ID,
                                   Topics.Title             AS Title,
                                   Topics.CreatorID         AS CreatorID,
                                   Topics.CategoryID        AS CategoryID,
                                   Topics.DateCreated       AS DateCreated,
                                   Topics.LastActivity      AS LastActivity,
                                   Topics.Description       AS Description,
                                   Topics.Reported          AS Reported,
                                   Topics.Closed            AS Closed,
                                   Users.Avatar             AS CreatorPicture,
                                   Users.Username           AS CreatorUsername,
                                   Categories.Name          AS CategoryName,
                                   Categories.Color         AS CategoryColor,
                                   (SELECT COUNT(*)
                                          FROM TopicFollowers
                                          WHERE TopicFollowers.TopicID = {topicID})
                                                            AS Followers,
                                   (SELECT COUNT(*)
                                    FROM TopicLikes
                                    WHERE TopicLikes.TopicID = {topicID})
                                                            AS Likes,
                                   (SELECT COUNT(*)
                                          FROM Comments
                                          WHERE Comments.TopicID = {topicID})
                                                            AS Replies,
                                   (SELECT CAST(COUNT(1) AS BIT)
                                           FROM TopicFollowers
                                           WHERE TopicFollowers.TopicID = {topicID} AND TopicFollowers.FollowerID = {Id})
                                                             AS CurrentUserFollows,
                                   (SELECT CAST(COUNT(1) AS BIT)
                                            FROM TopicLikes
                                            WHERE TopicLikes.TopicID = {topicID} AND TopicLikes.UserID = {Id})
                                                             AS CurrentUserLikes
                                   FROM Topics
                                   INNER JOIN Users ON Users.ID=Topics.CreatorID
                                   INNER JOIN Categories ON Categories.ID=Topics.CategoryID
                                   WHERE Topics.ID = {topicID}";

            var topic = _connection.Query<TopicDTO>(sqlQueryTopic).FirstOrDefault();
            return topic;
        }

        public IList<TopicDTO> GetTopics()
        {

            string sql = @"SELECT
                Topics.ID            AS ID,
                Topics.Title         AS Title,
                Topics.CreatorID     AS CreatorID,
                Topics.CategoryID    AS CategoryID,
                Topics.DateCreated   AS DateCreated,
                Topics.LastActivity  AS LastActivity,
                Topics.Description   AS Description,
                (SELECT COUNT(*)
                 FROM TopicLikes
                 WHERE TopicLikes.TopicID = Topics.ID)
                                     AS Likes,
                (SELECT COUNT(*)
                       FROM Comments
                       WHERE Comments.TopicID = Topics.ID)
                                     AS Replies,
                Topics.Reported      AS Reported,
                Topics.Closed        AS Closed,
                Users.Avatar         AS CreatorPicture,
                Users.Username       AS CreatorUsername,
                Categories.Name      AS CategoryName,
                Categories.Color     AS CategoryColor
                FROM Topics
                INNER JOIN Users ON Users.ID=Topics.CreatorID
                INNER JOIN Categories ON Categories.ID=Topics.CategoryID
                ORDER BY Topics.LastActivity DESC";

            var topics = _connection.Query<TopicDTO>(sql).ToList();

            return topics;
        }

        public IList<TopicDTO> GetTopicsByCategory(int categoryID)
        {
            string sql = $@"SELECT
                Topics.ID            AS ID,
                Topics.Title         AS Title,
                Topics.CreatorID     AS CreatorID,
                Topics.CategoryID    AS CategoryID,
                Topics.DateCreated   AS DateCreated,
                Topics.LastActivity  AS LastActivity,
                Topics.Description   AS Description,
                (SELECT COUNT(*)
                 FROM TopicLikes
                 WHERE TopicLikes.TopicID = Topics.ID)
                                     AS Likes,
                (SELECT COUNT(*)
                       FROM Comments
                       WHERE Comments.TopicID = Topics.ID)
                                     AS Replies,
                Topics.Reported      AS Reported,
                Topics.Closed        AS Closed,
                Users.Avatar         AS CreatorPicture,
                Users.Username       AS CreatorUsername,
                Categories.Name      AS CategoryName,
                Categories.Color     AS CategoryColor
                FROM Topics
                INNER JOIN Users ON Users.ID=Topics.CreatorID
                INNER JOIN Categories ON Categories.ID=Topics.CategoryID
                WHERE Topics.CategoryID = {categoryID}
                ORDER BY Topics.LastActivity DESC";

            var topics = _connection.Query<TopicDTO>(sql).ToList();

            return topics;
        }

        public void LikeTopic(TopicLike topicLike)
        {
            string query = @"INSERT INTO TopicLikes (TopicID, UserID)
                             values(@TopicID, @UserID)";
            _connection.Execute(query, new { topicLike.TopicID, topicLike.UserID });
        }

        public void UnlikeTopic(TopicLike topicLike)
        {
            string query = @"DELETE FROM TopicLikes 
                             WHERE TopicID = @TopicID 
                             AND UserID = @UserID";
            _connection.Execute(query, new { topicLike.TopicID, topicLike.UserID });
        }

        public void FollowTopic(TopicFollower topicFollower)
        {
            string query = @"INSERT INTO TopicFollowers (TopicID, FollowerID)
                             values(@TopicID, @FollowerID)";
            _connection.Execute(query, new { topicFollower.TopicID, topicFollower.FollowerID });
        }

        public void UnfollowTopic(TopicFollower topicFollower)
        {
            string query = @"DELETE FROM TopicFollowers 
                             WHERE TopicID = @TopicID 
                             AND FollowerID = @FollowerID";
            _connection.Execute(query, new { topicFollower.TopicID, topicFollower.FollowerID });
        }
    }
}