package funclib

import (
	"crypto/sha256"
	"encoding/json"
	"fmt"
	"net/http"

	// Libs
	"udlib/data"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)


// AUTH POST; It is used for accepting HTTP POST Method [creating new account]
func PostAuth(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  // Getting data from our request
  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  group := r.FormValue(data.GroupID)

  // Checking if some requests are NULL
  if username == "" || password == "" || group == "" {
    fmt.Printf("\nPOST username %s\npassword %s\ngroup %s\n", username, password, group)
    w.Write([]byte("NULL"))
    return
  }


  // DB Insert [using Prepare]
  cols := data.UserCols
  querry := fmt.Sprintf("INSERT INTO %s (%s, %s, %s) VALUES (?, ?, ?)", data.User_Table, cols.Login, cols.Password, cols.GroupID)

  userPOST, err := db.Prepare(querry)
  stat := FastErroring(err, "PostAuth: DB Prep | Error")
  defer fmt.Println("PostAuth: DB Prepare | Closed")
  defer userPOST.Close()
  if !stat { return }

  // Hash for password
  h := sha256.New()
  h.Write([]byte(password))
  hash := h.Sum(nil)

  // Writing into DB
  res, err := userPOST.Exec(username, hash, group)
  stat = FastErroring(err, "PostAuth: Exec | Some Issues")
  if !stat { return }
  if res == nil {
    fmt.Println("Test of NULL")
    w.Write([]byte("That Login already exists!"))
    return
  }
  w.Write([]byte("OK!")) // delete at some point

}

//// IN THE FUTURE
// GET AUTH; Generates token for current session
func GetAuth(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  userid := r.PathValue(data.ID)
  if userid == "" {
    http.Error(w, "NULL ID", http.StatusBadRequest)
    return
  }


}


// USER GET; It is used for getting HTTP GET Method
// Gives all info user has (eg. login, groupid and created)
func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  //// in the future - CHANGE TO TOKENS
  userid := r.PathValue(data.ID)

  // Making querry from "Template" [using Query]
  cols := data.UserCols
  querry := fmt.Sprintf("SELECT %s, %s, %s FROM %s WHERE ID=?", cols.Login, cols.GroupID, cols.Created, data.User_Table)

  rows, err := db.Query(querry, userid)
  stat := FastErroring(err, "GetUserInfo: DB Query | Error")
  defer fmt.Println("GetUserInfo: DB Query | Closed")
  defer rows.Close()
  if !stat { return }

  //var users []UserJSON
  var us data.UserJSON
  for rows.Next() {
    if err = rows.Scan(&us.User, &us.GroupID, &us.Created); err != nil {
      stat := FastErroring(err, "GetUserInfo: DB Prep | Some Issues")
      if !stat { return }
    }
    fmt.Println(us)
  }

  marsh, err := json.Marshal(us)
  stat = FastErroring(err, "GetUserInfo: JSON Marshal | Can't Marshal")
  if !stat { return }
  w.Write(marsh)
  fmt.Println("GET was Sent")

}


// GET COURSES; it is used to get all Groups from user
func GetCourses(w http.ResponseWriter, r *http.Request, db *sql.DB) {
  
  //// CHANGE TO TOKENS
  userid := r.PathValue(data.ID)

  c_cols := data.CourseCols
  u_cols := data.UserCols
  querry := fmt.Sprintf("SELECT c.%s FROM %s AS c INNER JOIN %s AS u ON c.%s=u.%s", c_cols.CoName, data.Course_Table, data.User_Table, c_cols.GroupID, u_cols.GroupID)

  db.Query(querry, userid)
}
