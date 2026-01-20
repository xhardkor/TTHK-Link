package funclib

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"

	// Libs
	"ml/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

// GET MESSAGES
func GetCourseMessages(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetCourseMessages: Request Body Closed")
  defer r.Body.Close()

  group_id := r.PathValue(data.GroupID)
  course_id := r.PathValue(data.CourseID)

  // TOKEN CHECKER => FUTURE

  m_cols := data.MessageCols
  c_cols := data.CourseCols
  u_cols := data.UserCols

  query := fmt.Sprintf(`
    SELECT u.%s, m.%s, m.%s, m.%s FROM %s AS m
    JOIN %s AS c ON m.%s=c.%s
    JOIN %s AS u ON m.%s=u.%s
    WHERE c.%s=? AND c.%s=?
  `, 
  u_cols.Login, m_cols.Msg, m_cols.Created, m_cols.ID, data.Message_Table,
  data.Course_Table, m_cols.CourseID, c_cols.ID,
  data.User_Table, m_cols.UserID, u_cols.ID,
  c_cols.GroupID, c_cols.CoName)

  row, err := db.Query(query, group_id, course_id)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetCourseMessages: DB Querry | Error ==> %s\n", err)
    return
  }
  defer fmt.Println("GetCourseMessages Row Closed")
  defer row.Close()

  // Creating and Using JSON format
  var msgs []data.MessageJSON
  for row.Next() {
    var msg data.MessageJSON
    if err := row.Scan(&msg.UserID, &msg.Msg, &msg.Created, &msg.ID); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| GetCourseMessages: JSON Prep | Error ==> %s\n", err)
      return
    }
    msgs = append(msgs, msg)
  }
  marsh, err := json.Marshal(msgs)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetCourseMessages: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.WriteHeader(http.StatusOK)
  w.Write(marsh)

}

