package funclib

import (
	"fmt"
	"net/http"
	"encoding/json"

	// Libs
	"ml/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)


func GetMessages(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetMessages: Request Body Closed")
  defer r.Body.Close()

  roomid := r.PathValue(data.RoomID)
  courseid := r.PathValue(data.CourseID)

//DB:
  u_cols := data.UserCols
  m_cols := data.MessageCols
  tmp := fmt.Sprintf(`
    SELECT m.%s, m.%s, m.%s, u.%s, m.%s, m.%s 
    FROM %s AS m 
    JOIN %s AS u ON m.%s=u.%s 
    WHERE m.%s=? AND m.%s=?;
  `,
  m_cols.ID, m_cols.Msg, m_cols.Created, u_cols.Login, m_cols.CourseID, m_cols.RoomID,
  data.Message_Table,
  data.User_Table, m_cols.UserID, u_cols.ID,
  m_cols.RoomID, m_cols.CourseID)

  rows, err := db.Query(tmp, roomid, courseid)
  if err!=nil {
    InternalError(w, "| GetMessages: DB Query | Error ==> %s\n", err)
    return
  }
  defer rows.Close()


  //JSON:
  var msgs []data.MessageJSON
  for rows.Next() {
    var msg data.MessageJSON
    if err:= rows.Scan(&msg.ID, &msg.Msg, &msg.Created, &msg.User.User, &msg.CourseID, &msg.RoomID); err!=nil {
      InternalError(w, "| GetMessages: JSON Scan | Error ==> %s\n", err)
      return
    }
    msgs = append(msgs, msg)
  }

  marsh, err := json.Marshal(msgs)
  if err!=nil {
    InternalError(w, "| GetMessages: JSON Marshal | Error ==> %s\n", err)
    return
  }
  
  InternalOK(w, marsh)
}
