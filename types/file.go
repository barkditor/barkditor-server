package types

type File struct {
	Filename  string `json:"filename"`
	Directory string `json:"directory"`
	Content   string `json:"content"`
}
