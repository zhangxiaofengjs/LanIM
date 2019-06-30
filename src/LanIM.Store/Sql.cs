using Com.LanIM.Store.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.LanIM.Store
{
    class Sql
    {
        public const string CREATE_DB = @"
            CREATE TABLE MSG_HISTORY_TBL(
                C_ID INTEGER       PRIMARY KEY AUTOINCREMENT
                                   NOT NULL,
                C_TYPE INT           NOT NULL,
                C_TIME DATETIME      NOT NULL,
                C_FROM_USER_ID CHAR (12)     NOT NULL,
                C_TO_USER_ID CHAR (12)     NOT NULL,
                C_CONTENT VARCHAR(512),
                C_FLAG         BOOLEAN     NOT NULL
                               DEFAULT (0)
            );

            CREATE TABLE CONTACTER_TBL(
                C_ID INTEGER  PRIMARY KEY AUTOINCREMENT
                              NOT NULL,
                C_NICK_NAME CHAR(20) NOT NULL,
                C_MAC       CHAR(12) NOT NULL,
                C_IP        CHAR(15)
            );";

        public const string QUERY_CONTACTER =
            @"SELECT C_ID, C_NICK_NAME, C_MAC, C_IP
              FROM CONTACTER_TBL";

        public const string QUERY_MAX_MESSAGE_ID =
            @"SELECT MAX(C_ID)
              FROM MSG_HISTORY_TBL";

        public const string QUERY_USER_LATEST_MESSAGE =
            @"SELECT C_ID, C_TYPE, C_TIME, C_FROM_USER_ID, C_TO_USER_ID, C_CONTENT,C_FLAG
              FROM MSG_HISTORY_TBL
              WHERE (C_FROM_USER_ID=@USER_ID OR C_TO_USER_ID=@USER_ID) AND C_ID < @ID
              ORDER BY C_ID DESC
              LIMIT 20";

        public const string ADD_MESSAGE =
            @"INSERT INTO MSG_HISTORY_TBL
              (C_ID, C_TYPE, C_TIME, C_FROM_USER_ID, C_TO_USER_ID, C_CONTENT, C_FLAG)
              VALUES
              (NULL, @C_TYPE, @C_TIME, @C_FROM_USER_ID, @C_TO_USER_ID, @C_CONTENT, @C_FLAG)";

        public const string UPDATE_MESSAGE_STATE =
           @"UPDATE MSG_HISTORY_TBL
             SET C_FLAG=@C_FLAG
             WHERE C_ID=@C_ID";
    }
}
