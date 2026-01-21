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


// Give all the user's info (eg. login, group_id[eg. "TITge24"] and date->created)
func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetUserInfo: Request Body Closed")
  defer r.Body.Close()

  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  var user_token []byte
  var tk Token = &tokenImpl{db: db, token: &user_token}
  tk.Get(r)

  var msg string
  var hash_password []byte
  var pswImpl Password = &passImpl{db: db, msg: &msg, hash_ps: &hash_password}
  pswImpl.Hash(password)

  good, err := pswImpl.Check(username, hash_password)
  if err!=nil {
    InternalError(w, "| GetUserInfo: PasswordCheck | Error ==> %s\n", err)
    return
  } else if !good {
    w.WriteHeader(http.StatusForbidden)
    log.Println("Password is wrong", msg, "|")
    return
  }
  
  //F: Check/Generate Token
  exist, err := tk.Check(username)
  if err!=nil {
    InternalError(w, "| GetUserInfo: Token Check | Error ==> %s\n", err)
    return
  }
  if !exist {
    if err := tk.Create(username, hash_password); err!=nil {
      InternalError(w, "| GetUserInfo: Token- Create | Error ==> %s\n", err)
      return
    }
  }

  //F: Making query
  u_cols := data.UserCols
  c_cols := data.CourseCols
  query := fmt.Sprintf(`
    SELECT u.%s, u.%s, u.%s, u.%s, c.%s, c.%s, c.%s, c.%s FROM %s AS u
    INNER JOIN %s AS c ON u.%s=c.%s
    WHERE u.%s=?
  `,
  u_cols.ID, u_cols.Login, u_cols.GroupID, u_cols.Created, c_cols.GroupID, c_cols.CoName, c_cols.Desc, c_cols.ID, data.User_Table,
  data.Course_Table, u_cols.GroupID, c_cols.GroupID,
  u_cols.Login)

  rows, err := db.Query(query, username)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: DB Query | Error ==> %s\n", err)
    return
  }
  defer fmt.Println("GetUserInfo: DB Query | Closed")
  defer rows.Close()


  //F: Creating and Using JSON format
  var us data.UserJSON
  var courses []data.CourseJSON
  hasRows := false
  for rows.Next() {
    hasRows=true
    var course data.CourseJSON
    if err = rows.Scan(&us.ID, &us.User, &us.GroupID, &us.Created, &course.GroupID, &course.CourseName, &course.Desc, &course.ID); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| GetUserInfo: Rows Scan | Error ==> %s\n", err)
      return
    }
    courses = append(courses, course)
  }
  if !hasRows {
    w.WriteHeader(http.StatusInternalServerError)
    return
  }
  if err:=rows.Err(); err!=nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: Rows Err | Error ==> %s\n", err)
    return
  }

  u_c_resp := struct{
    Token []byte `json:"token"`
    User data.UserJSON `json:"user"`
    Course []data.CourseJSON `json:"courses"`
  }{
    Token: user_token,
    User: us,
    Course: courses,
  }

  marsh, err := json.MarshalIndent(u_c_resp, "", "  ")
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.WriteHeader(http.StatusOK)
  w.Write(marsh)

}

