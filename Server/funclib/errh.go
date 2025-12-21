package funclib

import (
  "time"
  "fmt"
  "net/http"
)

// Func for Error test
func FastErroring(err error, txt string) {
  if err != nil {
    h, m, s := time.Now().Clock()
    fmt.Printf("%d:%d:%d|\n%s ==> %v\n",h,m,s , txt, err)
    http.NotFoundHandler()
  }
}
