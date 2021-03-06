<template>
  <div id="image-upload">
    <v-layout row justify-center>
    <v-dialog  v-model="dialog" max-width="500px">
      <v-btn small color="dark" dark slot="activator">Upload Image</v-btn>
      <v-card dark>
        <div id="success">
          <v-layout>
            <v-flex xs12>
              <v-alert type="success" :value="showSuccess">
              <span>
                {{ responseData }}
              </span>
              </v-alert>
            </v-flex>
          </v-layout>
        </div>
        <div v-show="showError" id="error-div">
          <v-layout>
          <v-flex xs12>
            <v-alert id="menu-error" :value=true icon='warning'>
              <span id="error-title">
                An error has occurred
              </span>
            </v-alert>
          </v-flex>
          </v-layout>
          <v-layout>
            <v-flex xs20>
              <v-card id="error-card">
                <p v-for="error in errors" :key="error">
                  {{ error }}
                </p>
              </v-card>
            </v-flex>
          </v-layout>
        </div>
        <br/>
        <v-flex xs4>
            <label class="custom-file-upload">
                <h5>CHOOSE IMAGE
                <i class="material-icons">cloud_download</i>
                </h5>
              <input id="uploadImage" name="imageInput" ref="imageData" type="file" @change="StoreSelectedFile" accept="image/*"/>
            </label>
            <v-spacer/>
            <v-btn small id="submitImage" name= "submitButton" color="pink" type="submit" value ="upload" v-on:click="submitImageUpload">
              Upload
            <v-icon color="white">cloud_upload</v-icon>
            </v-btn>
          </v-flex>
          <br/>
            <div class="preview-image" v-if="imageData.length > 0">
              <img id="previewImage" class="preview" :src="imageData"/>
            </div>
          <br/>
          </v-card>
          </v-dialog>
        </v-layout>
    <br/>
  </div>
</template>

<script>
import axios from 'axios'
import jwt from 'jsonwebtoken'
export default {
  name: 'ImageHome',
  dialog: false,
  components: {
  },
  data: () => ({
    selectedFile: null,
    responseData: '',
    show: false,
    test: null,
    showError: false,
    showSuccess: false,
    username: '',
    imageData: '' // Stores in base 64 format of image
  }),
  beforeCreate () {
    if (this.$store.state.authenticationToken === null) {
      this.$router.push({path: '/Unauthorized'})
    }
    try {
      var decodedToken = jwt.decode(this.$store.state.authenticationToken)
      if (!decodedToken.ReadUserProfile === 'True') {
        this.$router.push({path: '/Forbidden'})
      }
    } catch (ex) {
      this.$router.push({path: '/Forbidden'})
    }
  },
  methods: {
    StoreSelectedFile: function (event) {
      this.selectedFile = event.target.files[0]
      this.previewImage(event)
    },
    previewImage: function (event) {
      var input = event.target // References the DOM input element
      if (input.files[0]) {
        var reader = new FileReader() // Read image and convert to base64
        reader.onload = (image) => {
          this.imageData = image.target.result // Read image as base64
        }
        reader.readAsDataURL(input.files[0]) // Read as data url (base64 format)
      }
    },
    submitImageUpload: function () {
      // ReadRestaurantProfile
      var formData = new FormData()
      formData.append('username', this.$store.state.username)
      formData.append('filename', this.selectedFile, this.selectedFile.name)
      axios.post(this.$store.state.urls.profileManagement.profileImageUpload, formData, {
        headers: { Authorization: `Bearer ${this.$store.state.authenticationToken}` }
      }).then(response => {
        this.responseData = response.data
        this.showSuccess = true
        this.showError = false
        this.dialog = false
        this.getUserProfile()
        // get profile
      }).catch(error => {
        this.responseData = error.response.data
        this.showSuccess = false
        this.showError = true
        try {
          if (error.response.status === 401) {
            // Route to Unauthorized page
            this.$router.push('Unauthorized')
          }
          if (error.response.status === 403) {
            // Route to Forbidden page
            this.$router.push('Forbidden')
          }
          if (error.response.status === 404) {
            // Route to ResourceNotFound page
            this.$router.push('ResourceNotFound')
          }
          if (error.response.status === 500) {
            // Route to InternalServerError page
            this.$router.push('InternalServerError')
          } else {
            this.errors = JSON.parse(JSON.parse(error.response.data.message))
          }
          Promise.reject(error)
        } catch (ex) {
          this.errors = error.response.data
          Promise.reject(error)
        }
      })
    },
    getUserProfile () {
      axios.get(this.$store.state.urls.profileManagement.userProfile, {
        headers: {
          Authorization: `Bearer ${this.$store.state.authenticationToken}`
        }
      }).then(response => {
        this.displayName = response.data.displayName
        this.displayPicture = response.data.displayPicture
      }).catch(error => {
        try {
          if (error.response.status === 401) {
            // Route to Unauthorized page
            this.$router.push({ path: '/Unauthorized' })
          }
          if (error.response.status === 403) {
            // Route to Forbidden page
            this.$router.push({ path: '/Forbidden' })
          }
          if (error.response.status === 404) {
            // Route to ResourceNotFound page
            this.$router.push({ path: '/ResourceNotFound' })
          }
          if (error.response.status === 500) {
            // Route to InternalServerError page
            this.$router.push({ path: '/InternalServerError' })
          } else {
            this.errors = JSON.parse(JSON.parse(error.response.data.message))
          }
          Promise.reject(error)
        } catch (ex) {
          this.errors = error.response.data
          Promise.reject(error)
        }
      })
    }
  }
}
</script>

<style scoped>
#image-upload{
  width: 0px;
  height: 0px;
  border-top-width: 10px;
  padding-top: 0px;
  padding-bottom: 15px;
}
#previewImage{
  height: 100px;
  width: 100px;
}
input[type="file"] {
  display: none;
}
.custom-file-upload {
  display: inline-block;
  padding: 0em .5em .25em .25em;
  cursor: pointer;
  background: slateblue;
  font-size: 15px;
  margin-left: .30em;
  margin-right:.30em;
}
.btn--small{
  font: 5em;
}
#card {
  padding: 0 0.7em 0 0.7em;
  margin: 0 0 1em 0;
}
#user-text-box-alert{
  background-color: #e26161 !important
}
.btn__content{
    border-top-width: 10px;
    padding-top: 10px;
    padding-bottom: 10px;
    padding-right: 10px;
    padding-left: 10px;
    width: 137.5px;
    height: 27px;
}
/* img#display-picture{
  height: 50px;
  width:50px;
  padding-left:0px;
} */
</style>
