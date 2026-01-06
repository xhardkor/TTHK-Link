package funclib

import (
	"encoding/json"
	"fmt"
	"log"
	"net/http"
  "time"

	// Libs
	"udlib/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)


// AUTH POST; It is used for accepting HTTP POST Method [creating new account]
func PostAuth(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("PostAuth: Request Body Closed")
  defer r.Body.Close()

  // Getting data from our request
  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  group := r.FormValue(data.GroupID)
  //gr_admin := r.FormValue(data.SecretGroup)

  // Checking if some requests are NULL
  if username == "" || password == "" || group == "" {
    tmp := struct {
      Us    string  `json:"login"`
      Pass  string  `json:"password"`
      Gr    string  `json:"group"`
    }{
      Us: username,
      Pass: password,
      Gr: group,
    }
    w.WriteHeader(http.StatusConflict)

    enc := json.NewEncoder(w)
    if err := enc.Encode(&tmp); err != nil {
      log.Printf("| PostAuth: Some Error| Error ==> %s\n", err)
      return
    }
    return
  }


  // NEXT
  // Cheking if username already exists


  // Hash for password
  hash := CreateHashPsw(password)


  // DB Insert [using Prepare]
  cols := data.UserCols
  querry := fmt.Sprintf("INSERT INTO %s (%s, %s, %s, %s) VALUES (?, ?, ?, ?)", data.User_Table, cols.Login, cols.Password, cols.GroupID, cols.Created)

  stmt, err := db.Prepare(querry)
  if err != nil {
    log.Printf("| PostAuth: DB Prep | Error ==> %s\n", err)
    return
  }
  defer fmt.Println("| PostAuth: DB Prepare | Closed")
  defer stmt.Close()


  // Writing into DB
  res, err := stmt.Exec(username, hash, group, time.Now())
  if err != nil {
    log.Printf("| PostAuth: Exec | Some Issues ==> %s\n", err)
    return
  }
  if res == nil {
    fmt.Println("Test of NULL")
    w.Write([]byte("That Login already exists!"))
    return
  }

  // Automatically gives Token 
  token, err := GetToken(username, hash)
  tmp := struct{ Token []byte `json:"token"`}{Token: token}
  marsh, err := json.Marshal(tmp)
  if err != nil {
    log.Printf("| PostAuth: JSON Marshal | Error ==> %s", err)
    return
  }
  w.WriteHeader(http.StatusAccepted)
  w.Write(marsh)

}


// USER GET; It is used for getting HTTP GET Method
// Gives all info user has (eg. login, group_id[eg. "TITge24"] and date->created)
func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetUserInfo: Request Body Closed")
  defer r.Body.Close()

  token := r.FormValue(data.Token)
  // Token check 
  if token == "" {
    //getToken(w, r.FormValue(data.Login))
  }

  // Making querry from "Template" [using Query]
  cols := data.UserCols
  querry := fmt.Sprintf("SELECT %s, %s, %s FROM %s WHERE ID=?",
  cols.Login, cols.GroupID, cols.Created, data.User_Table)

  rows, err := db.Query(querry, token)
  if err != nil {
    log.Printf("| GetUserInfo: DB Query | Error ==> %s\n", err)
    return
  }
  defer fmt.Println("GetUserInfo: DB Query | Closed")
  defer rows.Close()


  // Creating and Using JSON format
  var us data.UserJSON
  for rows.Next() {
    if err = rows.Scan(&us.User, &us.GroupID, &us.Created); err != nil {
      log.Printf("| GetUserInfo: JSON Prep | Error ==> %s\n", err)
      return
    }
  }

  marsh, err := json.Marshal(us)
  if err != nil {
    log.Printf("| GetUserInfo: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.Write(marsh)

}


// GET COURSES; it is used to get all Groups from user
func GetCourses(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetCourses: Request Body Closed")
  defer r.Body.Close()

  userid := r.FormValue(data.Token)

  c_cols := data.CourseCols
  u_cols := data.UserCols

  querry := fmt.Sprintf(`
    SELECT c.%s FROM %s AS c 
    INNER JOIN %s AS u ON c.%s=u.%s AND u.%s=?
  `,
  c_cols.CoName, data.Course_Table, data.User_Table, c_cols.GroupID, u_cols.GroupID, u_cols.ID)

  row, err := db.Query(querry, userid)
  if err != nil {
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
      log.Printf("| GetCourses: JSON Prep | Error ==> %s\n", err)
      return
    }
    courses = append(courses, course)
  }

  marsh, err := json.Marshal(courses)
  if err != nil {
    log.Printf("| GetCourses: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.Write(marsh)
}


// GET MESSAGES
func GetCourseMessages(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetCourseMessages: Request Body Closed")
  defer r.Body.Close()

  group_id := r.PathValue(data.GroupID)
  course_id := r.PathValue(data.CourseID)

  // TOKEN CHECKER => FUTURE
  //user_token := r.FormValue(data.Token)

  m_cols := data.MessageCols
  c_cols := data.CourseCols
  u_cols := data.UserCols

  querry := fmt.Sprintf(`
    SELECT u.%s, m.%s, m.%s, m.%s FROM %s AS m
    JOIN %s AS c ON m.%s=c.%s
    JOIN %s AS u ON m.%s=u.%s
    WHERE c.%s=? AND c.%s=?
  `, 
  u_cols.Login, m_cols.Msg, m_cols.Created, m_cols.ID, data.Message_Table,
  data.Course_Table, m_cols.CourseID, c_cols.ID,
  data.User_Table, m_cols.UserID, u_cols.ID,
  c_cols.GroupID, c_cols.CoName)

  row, err := db.Query(querry, group_id, course_id)
  if err != nil {
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
      log.Printf("| GetCourseMessages: JSON Prep | Error ==> %s\n", err)
      return
    }
    msgs = append(msgs, msg)
  }
  marsh, err := json.Marshal(msgs)
  if err != nil {
    log.Printf("| GetCourseMessages: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.Write(marsh)

}


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

  querry := fmt.Sprintf(`
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

  rows, err := db.Query(querry, msg, group, course, token)
  if err != nil {
    log.Printf("| EditMessage: DB Query | Error ==> %s", err)
    return
  }
  defer fmt.Println("EditMessage Row Closed")
  defer rows.Close()

}
