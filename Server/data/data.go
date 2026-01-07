package data


// Setting variables
var DB_Dir string = "ignored/db_guard.json"
// Token Salt
const Salt = "SOME_OF_SALT"

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
  ID = "id"
  Login = "login"
  Password = "password"
  SecretGroup = "admin"
  GroupID = "group_id"
  Created = "created"
  CourseID = "course_id"
  Msg = "msg"
  Token = "token"
)

// Structs:

// Const variable for User_Table
const User_Table = "user_t"
type user_t struct {
  ID, Login, Password, IsAdmin, GroupID, Created string
}
var UserCols = user_t{
  ID: "ID",
  Login: "Login",
  Password: "Password",
  IsAdmin: "IsAdmin",
  GroupID: "GroupID",
  Created: "Created",
}


// Const variable for Course_Table
const Course_Table = "course_t"
type course_t struct {
  ID, GroupID, Desc, CoName string
}
var CourseCols = course_t{
  ID: "ID",
  GroupID: "GroupID",
  Desc: "Description",
  CoName: "CourseName",
}


// Message_Table
const Message_Table = "message_t"
type message_t struct {
  ID, Created, Msg, UserID, CourseID string
}
var MessageCols = message_t{
  ID: "ID",
  Created: "Created",
  Msg: "Msg",
  UserID: "UserID",
  CourseID: "CourseID",
}


// Token_Table
const Token_Table = "token_t"
type token_t struct {
  UserName, Token, Time string
}
var TokenCols = token_t{
  UserName: "UserName",
  Token: "Token",
  Time: "Time",
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
