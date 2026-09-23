class AddPasswordToUsers < ActiveRecord::Migration[7.2]
  def change
    add_column :users, :password, :string
    remove_column :users, :encrypted_password, :string
    remove_column :users, :salt, :string
  end
end
