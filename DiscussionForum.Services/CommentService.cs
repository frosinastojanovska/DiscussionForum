﻿using Dapper;
using DiscussionForum.Domain.DomainModel;
using DiscussionForum.Services.DTOs;
using DiscussionForum.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DiscussionForum.Services
{
    public class CommentService : ICommentService
    {
        private IDbConnection _connection { get; set; }

        public CommentService(IDbConnection connection)
        {
            _connection = connection;
        }

        public void EditComment(int topicID, string Id, string content, DateTime date)
        {
            var sql = $@"UPDATE Comments
                         SET Comments.Content = @Content, Comments.DateEdited = @Date
                         WHERE Comments.ID = @ID
                         UPDATE Topics
                         SET Topics.LastActivity = @Date
                         WHERE Topics.ID = {topicID}";
            _connection.Execute(sql, new { content, date, Id });
        }

        public void LikeComment(int Id, int commentId)
        {
            var sql = $@"INSERT INTO CommentLikes (CommentID, UserID)
                         values(@CommentID, @ID)";
            _connection.Execute(sql, new { commentId, Id });
        }

        public void UnlikeComment(int Id, int commentId)
        {
            var sql = $@"DELETE FROM CommentLikes 
                         WHERE CommentID = @CommentID
                         AND UserID = @ID";
            _connection.Execute(sql, new { commentId, Id });
        }

        public void CreateComment(Comment comment)
        {
            var sql = $@"INSERT INTO Comments (TopicID, CommenterID, Content, DateCreated, DateEdited)
                         values(@TopicID, @CommenterID, @Content, @DateCreated, @DateEdited)
                         UPDATE Topics
                         SET Topics.LastActivity = @DateCreated
                         WHERE Topics.ID = @TopicID";
            _connection.Execute(sql, new { comment.TopicID, comment.CommenterID, comment.Content, comment.DateCreated, comment.DateEdited });

        }

        public IList<CommentDTO> GetComments(int topicID, int currentUserId)
        {
            var sqlQueryComments = $@"SELECT
                                        Comments.ID              AS ID,
                                        Comments.TopicID         AS TopicID,
                                        Comments.CommenterID     AS CommenterID,
                                        Comments.Content         AS Content,
                                        Comments.DateCreated     AS DateCreated,
                                        Comments.DateEdited      AS DateEdited,
                                        Users.Avatar             AS CommenterPicture,
                                        Users.Username           AS CommenterUsername,
                                        (SELECT COUNT(*)
                                        FROM CommentLikes
                                        WHERE CommentLikes.CommentID = Comments.ID)
                                                                 AS Likes,
                                        (SELECT CAST(COUNT(1) AS BIT)
                                        FROM CommentLikes
                                        WHERE CommentLikes.CommentID = Comments.ID AND CommentLikes.UserID = {currentUserId})
                                                                 AS LikedByUser
                                        FROM Comments
                                        INNER JOIN Users ON Users.ID=Comments.CommenterID
                                        WHERE Comments.TopicID = {topicID}";

           var comments =  _connection.Query<CommentDTO>(sqlQueryComments).ToList();

            return comments;
        }

        public void ReportComment(CommentReport report)
        {
            string query = @"INSERT INTO CommentReports (CommentID, ReporterID, Reason, DateCreated)
                             values(@CommentID, @ReporterID, @Reason, @DateCreated)";
            _connection.Execute(query, new { report.CommentID, report.ReporterID, report.Reason, report.DateCreated });
        }

        public void DeleteComment(int commentId)
        {
            string query = @"DELETE FROM Comments 
                             WHERE ID = @CommentID
                             DELETE FROM CommentReports 
                             WHERE CommentID = @CommentID";
            _connection.Execute(query, new { commentId });
        }

        public void DeleteCommentReport(int reportID)
        {
            string query = @"DELETE FROM CommentReports 
                             WHERE ID = @ReportID";
            _connection.Execute(query, new { reportID });
        }

        public IList<CommentReportDTO> GetCommentReports()
        {
            string sql = @"SELECT 
                            CommentReports.ID               AS ID,
                            CommentReports.CommentID        AS CommentID,
                            CommentReports.ReporterID       AS ReporterID,
                            CommentReports.Reason           AS Reason,
                            CommentReports.DateCreated      AS DateCreated,
                            Comments.CommenterID            AS CommenterID,
                            Comments.Content                AS CommentContent,
                            Comments.DateCreated            AS CommentDateCreated,
                            Users.Username                  AS ReporterUsername,
                            Topics.ID                       AS TopicID,
                            Topics.Title                    AS TopicTitle
                            FROM CommentReports
                            INNER JOIN Users ON Users.ID=CommentReports.ReporterID
                            INNER JOIN Comments ON Comments.ID=CommentReports.CommentID
                            INNER JOIN Topics ON Topics.ID=Comments.TopicID
                            ORDER BY CommentReports.DateCreated DESC";

            var reports = _connection.Query<CommentReportDTO>(sql).ToList();
            return reports;
        }
    }
}
