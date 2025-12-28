package data


// Setting variables
var DB_Dir string = "ignored/db_guard.json"





// Global variables

const (
  // for FormValue variable
  Login = "login"
  // for FormValue variable
  Password = "password"
  // for FormValue variable
  GroupID = "groupid"
  // for FormValue variable
  ID = "id"
)



// Structs:

// DB config struct
type DBConfig struct {
  DBUser string `json:"db_user"`
  DBPass string `json:"db_pass"`
  DBHost string `json:"db_host"`
  DBPort int    `json:"db_port"`
  DBName string `json:"db_name"`
}

// User JSON struct
type UserJSON struct {
  ID        int     `json:"id,omitempty"`
  User      string  `json:"user,omitempty"`
  Password  string  `json:"password,omitempty"`
  IsAdmin   bool    `json:"is_admin,omitempty"`
  GroupID   string  `json:"group_id,omitempty"`
  Created   string  `json:"created,omitempty"`
}

// Defining user_t table for convinience [private because of low key]
type user_col struct {
  ID, Login, Password, IsAdmin, GroupID, Created string
}
// Defined variables for user_t [public because of high key]
var UserCols = user_col{ID: "ID", Login: "Login", Password: "Password", IsAdmin: "IsAdmin", GroupID: "GroupID", Created: "Created"}
// Const variable for User_Table
const User_Table = "user_t"


// Defining course_t table for convinience [private because of low key]
type course_col struct {
  ID, GroupID, Desc, CoName string
}
// Defined variables for course_t [public because of high key]
var CourseCols = course_col{ID: "ID", GroupID: "GroupID", Desc: "Description", CoName: "CourseName"}
// Const variable for User_Table
const Course_Table = "course_t"
