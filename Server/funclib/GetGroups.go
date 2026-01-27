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


func GetGroups(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetGroups: Request Body Closed")
  defer r.Body.Close()

//DB:
  c_cols := data.CourseCols
  query := fmt.Sprintf("SELECT DISTINCT %s FROM %s", c_cols.GroupID, data.Course_Table)
  rows, err := db.Query(query)
  if err!=nil {
    InternalNULL(w)
    log.Println("No rows")
    return
  }
  defer rows.Close()

//JSON:
  var groups []data.CourseJSON
  if rows.Next() {
    var group data.CourseJSON
    fmt.Println(group)
    if err := rows.Scan(&group.GroupID); err!=nil {
      log.Println("nothing")
      InternalNULL(w)
      return
    }
    groups = append(groups, group)
  }

  fmt.Println(groups)
  marsh, err := json.Marshal(groups)
  if err!=nil {
    InternalError(w, "| GetGroups: JSON Marshal | Error ==> %s\n", err)
    return
  }
  InternalOK(w, marsh)
}
