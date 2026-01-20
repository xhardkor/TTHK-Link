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

// GET COURSES; it is used to get all Courses from user
func GetCourses(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetCourses: Request Body Closed")
  defer r.Body.Close()

  token := r.FormValue(data.Token)
  //username := r.FormValue(data.Login)
  /*
  _, ex := CheckToken(username, []byte(token), db)
  // NOT FINISHED
  if !ex {
    w.WriteHeader(http.StatusInternalServerError)
    return
  }*/

  c_cols := data.CourseCols
  u_cols := data.UserCols

  query := fmt.Sprintf(`
    SELECT c.%s FROM %s AS c 
    INNER JOIN %s AS u ON c.%s=u.%s AND u.%s=?
  `,
  c_cols.CoName, data.Course_Table, data.User_Table, c_cols.GroupID, u_cols.GroupID, u_cols.ID)

  row, err := db.Query(query, token)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetCourses: DB Querry | Error ==> %s\n", err)
    return 
  }
  defer fmt.Println("GetCourses Row Closed")
  defer row.Close()


  // Creating and Using JSON format
  var courses []data.CourseJSON
  for row.Next() {
    var course data.CourseJSON
    if err := row.Scan(&course.CourseName); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| GetCourses: JSON Prep | Error ==> %s\n", err)
      return
    }
    courses = append(courses, course)
  }

  marsh, err := json.Marshal(courses)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetCourses: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.WriteHeader(http.StatusOK)
  w.Write(marsh)
}

