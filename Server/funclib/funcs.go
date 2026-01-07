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

    enc := json.NewEncoder(w)
    if err := enc.Encode(&tmp); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| PostAuth: Encoder | Error ==> %s\n", err)
      return
    }
    w.WriteHeader(http.StatusBadRequest)
    return
  }

  // Cheking if Username already exists
  ex, err := UserCheck(username, db)
  if err != nil {
    log.Printf("| PostAuth: Username check | Error ==> %s\n", err)
    return
  }
  if !ex {
    w.WriteHeader(http.StatusBadRequest)
    return
  }

  // Hash for password
  hash := CreateHashPsw(password)

  // Insert data into DB
  cols := data.UserCols
  query := fmt.Sprintf("INSERT INTO %s (%s, %s, %s, %s) VALUES (?, ?, ?, ?)", data.User_Table, cols.Login, cols.Password, cols.GroupID, cols.Created)

  res, err := db.Exec(query, username, hash, group, time.Now())
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: DB Exec | Error ==> %s\n", err)
    return
  }
  rowAdd, err := res.RowsAffected()
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: Exec RowsAdded | Error ==> %s\n", err)
    return
  }
  if rowAdd<=0 {
    w.WriteHeader(http.StatusNoContent)
    return
  }

  // Automatically gives Token 
  token, err := GetToken(username, hash)
  tmp := struct{ Token []byte `json:"token"`}{Token: token}
  marsh, err := json.Marshal(tmp)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| PostAuth: JSON Marshal | Error ==> %s", err)
    return
  }
  w.WriteHeader(http.StatusCreated)
  w.Write(marsh)

}


// Gives all info user has (eg. login, group_id[eg. "TITge24"] and date->created)
func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetUserInfo: Request Body Closed")
  defer r.Body.Close()

  user := r.FormValue(data.Login)
  pswd := r.FormValue(data.Password)
  hs_psw := CreateHashPsw(pswd)

  // Generate Token
  token, err := GetToken(user, hs_psw)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: Token | NULL's ==> %s", err)
    return
  }

  // Making query from "Template" [using Query]
  cols := data.UserCols
  query := fmt.Sprintf("SELECT %s, %s, %s FROM %s WHERE ID=?",
  cols.Login, cols.GroupID, cols.Created, data.User_Table)

  rows, err := db.Query(query, token)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: DB Query | Error ==> %s\n", err)
    return
  }
  defer fmt.Println("GetUserInfo: DB Query | Closed")
  defer rows.Close()


  // Creating and Using JSON format
  var us data.UserJSON
  for rows.Next() {
    if err = rows.Scan(&us.User, &us.GroupID, &us.Created); err != nil {
      w.WriteHeader(http.StatusInternalServerError)
      log.Printf("| GetUserInfo: JSON Prep | Error ==> %s\n", err)
      return
    }
  }

  marsh, err := json.Marshal(us)
  if err != nil {
    w.WriteHeader(http.StatusInternalServerError)
    log.Printf("| GetUserInfo: JSON Marshal | Error ==> %s\n", err)
    return
  }
  w.WriteHeader(http.StatusOK)
  w.Write(marsh)

}


// GET COURSES; it is used to get all Courses from user
func GetCourses(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  defer fmt.Println("GetCourses: Request Body Closed")
  defer r.Body.Close()

  token := r.FormValue(data.Token)
  username := r.FormValue(data.Login)
  _, ex := CheckToken(username, []byte(token), db)
  // NOT FINISHED
  if !ex {
    w.WriteHeader(http.StatusInternalServerError)
    return
  }

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
