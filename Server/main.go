package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"net/http"
	"os"
	//"os/exec"

	// Websocket
	//"github.com/gorilla/websocket"

	// Config Directories
	"udlib/data"
  "udlib/funclib"

	// Database
	"database/sql"
	_ "github.com/go-sql-driver/mysql"
)



func main() {
  // JSON file reading and Formating
  readFile, err := os.ReadFile(data.DB_Dir)
  funclib.FastErroring(err, "MAIN: OS | Can't open and read file")

  var dbConf data.DBConfig
  err = json.Unmarshal(readFile, &dbConf)
  funclib.FastErroring(err, "MAIN: JSON | Can't input JSON format into struct")


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  funclib.FastErroring(err, "MAIN: DB | Connect error")
  err = db.Ping()
  funclib.FastErroring(err, "MAIN: DB | Ping error")
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()



  // Server Handler and Listener 
  mux := http.NewServeMux()

  // creating new auth
  mux.HandleFunc("POST /auth", func(w http.ResponseWriter, r *http.Request) {
    funclib.PostAuth(w, r, db)
  })
  // give id and token(- / ?)
  mux.HandleFunc("GET /auth", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetAuth(w, r, db) // NEXT
  })
  // give all info user has (w/o courses)
  mux.HandleFunc("GET /user/{token}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetUserInfo(w, r, db)
  })
  // give all Courses from a user(id)
  mux.HandleFunc("GET /courses/{token}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetCourses(w, r, db)
  })
  // give all messages of Group Course
  mux.HandleFunc("GET /messages/{group_id}/{course_id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetCourseMessages(w, r, db)
  })
  // write a message 
  mux.HandleFunc("POST /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })
  // create a new group
  mux.HandleFunc("POST /group", func(w http.ResponseWriter, r *http.Request) {
  })


  http.ListenAndServe(":8080", mux)
}


