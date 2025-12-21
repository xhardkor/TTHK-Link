package funclib

import (
  "fmt"
  "net/http"
  "crypto/sha256"
  "encoding/json"

  // Libs
  "udlib/data"

  // Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
  
)

// AUTH POST; It is used for accepting HTTP POST Method [creating new account]
func PostAuth(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  // DB Insert
  userPOST, err := db.Prepare("INSERT INTO user_t (Login, Password, GroupID) VALUES (?, ?, ?)")
  FastErroring(err, "POST: DB Prep | Error")
  defer fmt.Println("POST: DB Prepare | Closed")
  defer userPOST.Close()

  username := r.FormValue(data.Login)
  password := r.FormValue(data.Password)
  group := r.FormValue(data.Groupid)


  if username == "" || password == "" || group == "" {
    fmt.Printf("\nPOST username %s\npassword %s\ngroup %s\n", username, password, group)
    w.Write([]byte("NULL"))
    return
  }

  // Hash function
  h := sha256.New()
  h.Write([]byte(password))
  hash := h.Sum(nil)

  // Writing into DB
  res, err := userPOST.Exec(username, hash, group)
  FastErroring(err, "POST: Exec | Some Issues")
  if res == nil {
    fmt.Println("Test of NULL")
    w.Write([]byte("That Login already exists!"))
    return
  }
  w.Write([]byte("OK!"))

}



// USER GET; It is used for getting HTTP GET Method
func GetUserInfo(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  userid := r.PathValue(data.Id)
  if userid == "" {
    http.Error(w, "no ID", http.StatusBadRequest)
    return
  }

  // Making querry from "Template"
  table := data.User_t
  querry := fmt.Sprintf("SELECT %s, %s FROM %s WHERE ID=?", table.Login, table.GroupID, data.UserTable)
  rows, err := db.Query(querry, userid)
  FastErroring(err, "GET: DB Query | Error")
  defer fmt.Println("GET: DB Prepare | Closed")
  defer rows.Close()

  //var users []UserJSON
  var us data.UserJSON
  for rows.Next() {
    if err = rows.Scan(&us.User, &us.GroupID); err != nil {
      FastErroring(err, "GET: DB Prep | Some Issues")
    }
    fmt.Println(us)
  }

  marsh, err := json.Marshal(us)
  FastErroring(err, "GET: JSON Marshal | Can't Marshal")
  w.Write(marsh)
  fmt.Println("GET was Sent")

}

