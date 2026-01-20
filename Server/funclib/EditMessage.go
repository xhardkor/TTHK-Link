package funclib

import (
	"fmt"
	"log"
	"net/http"

	// Libs
	"ml/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)

// EDIT MESSAGE
func EditMessage(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("EditMessage Body Closed")
  defer r.Body.Close()

  group := r.PathValue(data.GroupID)
  course := r.PathValue(data.CourseID)

  msg := r.FormValue(data.Msg)
  token := r.FormValue(data.Token)

  m_cols := data.MessageCols
  c_cols := data.CourseCols
  u_cols := data.UserCols

  query := fmt.Sprintf(`
    UPDATE %s AS m
    JOIN %s AS u ON m.%s=u.%s
    JOIN %s AS c ON m.%s=c.%s
    SET m.%s=?
    WHERE u.%s=? AND c.%s=? AND u.%s=?
  `, 
  data.Message_Table,
  data.User_Table, m_cols.UserID, u_cols.ID,
  data.Course_Table, m_cols.CourseID, c_cols.ID,
  m_cols.Msg,
  u_cols.GroupID, c_cols.CoName, u_cols.Login)

  res, err := db.Exec(query, msg, group, course, token)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| EditMessage: DB Exec | Error ==> %s", err)
    return
  }
  rowAdd, err := res.RowsAffected()
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| EditMessage: Exec RowsAdded | Error ==> %s", err)
    return
  }
  if rowAdd<=0 {
    w.WriteHeader(http.StatusNoContent)
    return
  }

  w.WriteHeader(http.StatusAccepted)
}

