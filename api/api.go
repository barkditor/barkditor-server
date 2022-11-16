package api

import (
	"io"
	"os"
	"path/filepath"

	"github.com/barkditor/barkditor-server/types"
)

type API struct{}

func (API) Heartbeat(_ string, alive *bool) error {
	*alive = true
	return nil
}

func (API) Save(file types.File, success *bool) error {
	absolute := filepath.Join(file.Directory, file.Filename)

	err := os.WriteFile(absolute, []byte(file.Content), 0644)
	if err != nil {
		*success = false
		return err
	}

	*success = true
	return nil
}

func (API) Open(path string, file *types.File) error {
	directory := filepath.Dir(path)
	filename := filepath.Base(path)

	f, err := os.OpenFile(path, os.O_CREATE|os.O_RDONLY, 0644)
	if err != nil {
		return err
	}
	defer f.Close()

	content, err := io.ReadAll(f)
	if err != nil {
		return nil
	}

	*file = types.File{
		Filename:  filename,
		Directory: directory,
		Content:   string(content),
	}

	return nil
}
