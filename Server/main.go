package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"os"
	"time"

	// Websocket
	//"github.com/gorilla/websocket"

	// DataBase
	"database/sql"

	_ "github.com/go-sql-driver/mysql"
)

// Func for Error test
func FastErroring(err error, txt string) {
  if err != nil {
    h, m, s := time.Now().Clock()
    fmt.Printf("%d:%d:%d| %s ==> %v\n",h,m,s , txt, err)
    os.Exit(1)
  }
}



type DBConfig struct {
  DBUser string `json:"db_user"`
  DBPass string `json:"db_pass"`
  DBHost string `json:"db_host"`
  DBPort int    `json:"db_port"`
  DBName string `json:"db_name"`
}

func main() {

  // File Reading
  file, err := os.Open("db_guard.json")
  FastErroring(err, "MAIN: OS | Can't open db_guard.json file")
  defer fmt.Println("MAIN: OS | File closed")
  defer file.Close()

  readFile, err := io.ReadAll(file)


  // JSON Formating
  var dbConf DBConfig
  err = json.Unmarshal(readFile, &dbConf)
  FastErroring(err, "MAIN JSONf | Can't input json format into struct")


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  FastErroring(err, "MAIN: DB | Connect error")
  err = db.Ping()
  FastErroring(err, "MAIN: DB | Ping error")
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()


  // Server Handler and Listener 
  http.HandleFunc("/pepe", func(w http.ResponseWriter, r *http.Request) {
    handler(w, r, db)
  })
  http.ListenAndServe(":8080", nil)
}



// Main requests/responses functions
func handler(w http.ResponseWriter, r *http.Request, db *sql.DB) {

  // REST API
  switch r.Method {
  case "POST":
    userPOST, err := db.Prepare("INSERT INTO user (UserName, Password, IsAdmin) VALUES (?, ?, ?)")
    FastErroring(err, "POST: DB Prep | Error")
    defer fmt.Println("POST: DB Prepare | Closed")
    defer userPOST.Close()

    username := r.FormValue("name")
    password := r.FormValue("passwd")
    admin_s := r.FormValue("admin")

    admin_v := 0
    if admin_s == "true" || admin_s == "1" { 
      admin_v = 1
    }

    _, err = userPOST.Exec(username, password, admin_v)
    FastErroring(err, "POST: DB Prep | Some Issues")



  case "GET":
    // Don't work right now
    w.Header().Set("Content-Type","application/json")

  default:
    break
  }

}

