var email=document.getElementById('email');
var password=document.getElementById('password');
var form=document.forms[0];
var emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
function validate(){
    var correct=true;
if(email.value==""){
    setError(email,"Email Cannot be empty");
    // alert('hi1');
    correct=false
}
else if(!emailRegex.test(email.value)){
    setError(email,"Please enter a valid email");
    correct= false;
}
else
setSuccess(email);

if(password.value==""){
    setError(password,"Password Cannot be empty");
    correct= false;
}
else
setSuccess(password);
if(correct)
form.submit();
else 
return correct;
}
function setError(element,messg){
element.nextElementSibling.nextElementSibling.textContent=messg;

}
function setSuccess(element){
    element.nextElementSibling.nextElementSibling.textContent=""; 
}