package data


// Setting variables
var DB_Dir string = "ignored/db_guard.json"

// DB config struct
type DBConfig struct {
  DBUser string `json:"db_user"`
  DBPass string `json:"db_pass"`
  DBHost string `json:"db_host"`
  DBPort int    `json:"db_port"`
  DBName string `json:"db_name"`
}

// Global variables for FormValue variable
const (
  Login = "login"
  Password = "password"
  GroupID = "group_id"
  ID = "id"
  Token = "token"
  CourseID = "course_id"
  Msg = "msg"
)
const SecretGroup = "admin"

// Structs:



// Const variable for User_Table
const User_Table = "user_t"
// Defining user_t table for convinience [private because of low key]
type user_col struct {
  ID, Login, Password, IsAdmin, GroupID, Created string
}
// Defined variables for user_t [public because of high key]
var UserCols = user_col{
  ID: "ID",
  Login: "Login",
  Password: "Password",
  IsAdmin: "IsAdmin",
  GroupID: "GroupID",
  Created: "Created",
}


// Const variable for User_Table
const Course_Table = "course_t"
// Defining course_t table for convinience [private because of low key]
type course_col struct {
  ID, GroupID, Desc, CoName string
}
// Defined variables for course_t [public because of high key]
var CourseCols = course_col{
  ID: "ID",
  GroupID: "GroupID",
  Desc: "Description",
  CoName: "CourseName",
}


// Const variable for User_Table
const Message_Table = "message_t"
// Defining message_t table for convinience [private because of low key]
type message_col struct {
  ID, Created, Msg, UserID, CourseID string
}
// Defined variables for message_t [public because of high key]
var MessageCols = message_col{
  ID: "ID",
  Created: "Created",
  Msg: "Msg",
  UserID: "UserID",
  CourseID: "CourseID",
}

// Easy Requests
type RequestData struct {
  user user_col
}


// JSON Structs:

type UserJSON struct {
  ID        int     `json:"id,omitempty"`
  User      string  `json:"user,omitempty"`
  Password  string  `json:"password,omitempty"`
  IsAdmin   bool    `json:"is_admin,omitempty"`
  GroupID   string  `json:"group_id,omitempty"`
  Created   string  `json:"created,omitempty"`
}
type CourseJSON struct {
  ID          int     `json:"id,omitempty"`
  GroupID     string  `json:"group_id,omitempty"`
  Desc        string  `json:"desc,omitempty"`
  CourseName  string  `json:"name,omitempty"`
}
type MessageJSON struct {
  ID          int     `json:"id,omitempty"`
  Created     string  `json:"created,omitempty"`
  Msg         string  `json:"desc,omitempty"`
  CourseName  string  `json:"name,omitempty"`
  UserID      string  `json:"user_id,omitempty"`
  GroupID     string  `json:"group_id,omitempty"`
}
