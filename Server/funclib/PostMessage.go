package funclib

import (
	"encoding/json"
	"fmt"
	"time"

	//"log"
	"net/http"

	// Libs
	"ml/data"

	// Database
	"database/sql"

	_ "github.com/go-sql-driver/mysql"
)



func PostMessage(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("PostMessage: Request Body Closed")
  defer r.Body.Close()
  fmt.Println(r.Body)

//F:
  var tmp data.MessageJSON
  if err := json.NewDecoder(r.Body).Decode(&tmp); err!=nil {
    InternalError(w, "| PostMessage: Unable to Decode JSON | Error ==> %s\n", err)
  }

  now := time.Now().UTC()


//F:
  m_cols := data.MessageCols
  tmp2 := fmt.Sprintf(`
    INSERT INTO %s (%s, %s, %s, %s, %s)
    VALUES (?, ?, ?, ?, ?)
  `, data.Message_Table, m_cols.Created, m_cols.Msg, m_cols.UserID, m_cols.CourseID, m_cols.RoomID)

  res, err := db.Exec(tmp2, now, tmp.Msg, tmp.UserID, tmp.CourseID, tmp.RoomID)
  if err!=nil {
    InternalError(w, "| PostMessage: Unable to write into SQL | Error ==> %s\n", err)
  }
  fmt.Println("res: ", res)

}
