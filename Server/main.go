package main

import (
	// Standard Libraries
	"encoding/json"
	"fmt"
	"log"
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
  if err != nil {
    log.Printf("MAIN: OS | Can't open and read file ==> %s", err)
    return
  }

  var dbConf data.DBConfig
  if err := json.Unmarshal(readFile, &dbConf); err != nil {
    log.Printf("MAIN: JSON Unmarshal | Error ==> %s", err)
    return
  }


  // Connect to DB (DSN = Data Source Name)
  dsn := fmt.Sprintf("%s:%s@tcp(%s:%d)/%s?parseTime=true&timeout=5s", 
      dbConf.DBUser, dbConf.DBPass, dbConf.DBHost, dbConf.DBPort, dbConf.DBName,
  )
  db, err := sql.Open("mysql", dsn)
  if err != nil {
    log.Printf("MAIN: DB Connect | Error ==> %s", err)
    return
  }
  if err := db.Ping(); err != nil {
    log.Printf("MAIN: DB Ping | Error ==> %s", err)
    return
  }
  fmt.Println("MAIN: DB | Connected!")
  defer fmt.Println("MAIN: DB | closed")
  defer db.Close()



  // Server Handler and Listener 
  mux := http.NewServeMux()

  // creating new auth
  mux.HandleFunc("POST /auth/create", func(w http.ResponseWriter, r *http.Request) {
    funclib.PostAuth(w, r, db)
  })
  // give id and token
  mux.HandleFunc("GET /auth", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetAuth(w, r, db) // NEXT
  })
  // give all info user has (w/o courses)
  mux.HandleFunc("GET /user/info", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetUserInfo(w, r, db)
  })
  // give all Courses from a user(id)
  mux.HandleFunc("GET /user/courses", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetCourses(w, r, db)
  })
  // give all messages of Group Course
  mux.HandleFunc("GET /messages/{group_id}/{course_id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.GetCourseMessages(w, r, db)
  })
  // edit a message 
  mux.HandleFunc("PATCH /message/{group_id}/{course_id}", func(w http.ResponseWriter, r *http.Request) {
    funclib.EditMessage(w, r, db)
  })
  // write a message 
  mux.HandleFunc("POST /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })
  // delete a message 
  mux.HandleFunc("DELETE /message/{id}", func(w http.ResponseWriter, r *http.Request) {
  })


  http.ListenAndServe(":8080", mux)
}


